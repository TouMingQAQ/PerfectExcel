using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using PerfectExcel.Attribute;
using PerfectExcel.Convert;
using UnityEngine;
using UnityEngine.Pool;
using Object = System.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace PerfectExcel.Core
{
    public class ExcelReader
    {
        private const string DEFAULT_SPECIAL_CHARS = @"#$^&*|.<>?~";
        private ExcelPackage package;
        private Dictionary<string, ExcelWorksheet> workSheetMap;
        private string directoryPath;
        /// <summary>
        /// 正则匹配工作簿名
        /// </summary>
        private string regexValue;

        public ExcelReader(string directoryPath,string regexValue = ".*")
        {
            this.directoryPath = directoryPath;
            this.regexValue = regexValue;
            workSheetMap = new();
            LoadExcel();
        }

        public void LoadExcel()
        {
            workSheetMap ??= new();
            ReadExcel();
        }

        public void Read<TKey, TValue>(BaseExcelConfig<TKey, TValue> config)
        {
            if (package == null)
            {
                LogW($"Package is null , check excel file");
                return;
            }
            if (config == null)
            {
                LogE($"Read config is null");
                return;
            }

            var workSheetName = config.workName;
            if (string.IsNullOrEmpty(workSheetName))
            {
                LogE($"config`s workName is null");
                return;
            }

            if (!workSheetMap.TryGetValue(workSheetName, out var worksheet))
            {
                LogW($"Can`t find workSheet:{workSheetName}");
                return;
            }
        
            var dimension = worksheet.Dimension;
            if (dimension == null)
            {
                LogW($"Worksheet data is null:{workSheetName}");
                return;
            }
            int startRow = dimension.Start.Row; // 起始行（默认 1）
            int endRow = dimension.End.Row;     // 结束行（有数据的最后一行）
            int startCol = dimension.Start.Column; // 起始列（默认 1）
            int endCol = dimension.End.Column;     // 结束列（有数据的最后一列）
            string[,] valueArray = new string[endCol-startCol+1,endRow-startRow+1];
            for (int col = startCol; col <= endCol; col++)
            {
                for (int row = startRow; row <= endRow; row++)
                {

                    // 读取单元格值（Text 自动转换为字符串，兼容数字、日期、文本等）
                    string cellValue = worksheet.Cells[row, col].Text;
                    valueArray[col-startCol, row-startRow] = cellValue;
                }
            }
            //处理数据
            //去掉第一行和注释字段
            List<List<string>> valueList = ListPool<List<string>>.Get();
            int valueCount = valueArray.GetLength(1)-1;//数据大小(去除Title)
            int maxX = valueArray.GetLength(0);
            int maxY = valueArray.GetLength(1);
            for (int x = 0; x <maxX; x++)
            {
                var title = valueArray[x, 0];
                if(string.IsNullOrEmpty(title))
                    continue;
                if(StartsWithSpecialChar(title))
                    continue;
                List<string> values = ListPool<string>.Get();
                for (int y = 1; y < maxY; y++)
                {
                    var value = valueArray[x, y];
                    if (x == 0 && string.IsNullOrEmpty(value))
                    {
                        maxY = y;
                        continue;
                    }
                    values.Add(value);
                }
                valueList.Add(values);
            }
            //重新把数据排列放入ValueArray
            maxY--;
            maxX = valueList.Count;
            valueArray = new String[maxY,maxX];
            for (int i = 0; i < maxX; i++)
            {
                var list = valueList[i];
                for (int j = 0; j < list.Count; j++)
                {
                    var value = list[j];
                    valueArray[j, i] = value;
                }
            }

            var valueType = typeof(TValue);
            //Todo::根据反射获取TValue的数据和标签，识别Excel标签并根据标签的信息转换数据生成TValue
            config.valueList.Clear();
            var fields = valueType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (fields.Length != valueArray.GetLength(1))
            {
                LogE($"Convert error,[{valueType}] Fields Length {fields.Length} !=  {valueArray.GetLength(1)}");
                return;
            }
            for (int x = 0; x < valueArray.GetLength(0); x++)
            {
                var value = Activator.CreateInstance<TValue>();
                for (int i = 0; i < fields.Length; i++)
                {
                    var fieldInfo = fields[i];
                    var attribute = fieldInfo.GetCustomAttribute<ExcelAttribute>();
                    
                    var valueField = new ValueField
                    {
                        value = valueArray[x, i],
                        fieldInfo = fieldInfo,
                        attribute = attribute,
                        convertValue = null
                    };
                    var otherAttributes = fieldInfo.GetCustomAttributes();
                    foreach (var otherAttribute in otherAttributes)
                    {
                        if (otherAttribute is IExcelConvertValue convertValue)
                        {
                            valueField.convertValue = convertValue;
                            break;
                        }
                    }
                    var res = valueField.CreateConvert();
                    if (!res)
                    {
                        LogE($"  [<color=red>{valueField.value}</color>]   Convert error");
                        return;
                    }

                    value = (TValue)valueField.Convert(value);
                }
                config.valueList.Add(value);
            }
        }

        struct ValueField
        {
            public FieldInfo fieldInfo;
            public ExcelAttribute attribute;
            public IExcelConvertValue convertValue;
            public string value;
            public IStringConvert convert;

            public bool CreateConvert()
            {
                var convertType = attribute.convertType;
                if (convertType != null && convertType.GetInterface(nameof(IStringConvert)) != null)
                {
                    convert = Activator.CreateInstance(convertType) as IStringConvert;
                    return true;
                }
                var fieldType = fieldInfo.FieldType;
                if (fieldType == typeof(string))
                    convert = new ToStr();
                else if (fieldType == typeof(int))
                    convert = new ToInt32();
                else if (fieldType == typeof(long))
                    convert = new ToInt64();
                else if (fieldType == typeof(short))
                    convert = new ToInt16();
                else if (fieldType == typeof(uint))
                    convert = new ToUInt32();
                else if (fieldType == typeof(ulong))
                    convert = new ToUInt64();
                else if (fieldType == typeof(ushort))
                    convert = new ToUInt16();
                else if (fieldType == typeof(float))
                    convert = new ToFloat();
                else if (fieldType == typeof(double))
                    convert = new ToDouble();
                else if (fieldType == typeof(bool))
                    convert = new ToBool();
                else
                    return false;
                return true;
            }
            public object Convert(Object obj)
            {
                if (string.IsNullOrEmpty(value))
                    value = "";
                var valueObj = convert.Convert(value,convertValue);
                fieldInfo.SetValue(obj,valueObj);
                return obj;
            }
        }
        /// <summary>
        /// 判断字符串是否以特殊字符开头
        /// </summary>
        /// <param name="input">要判断的字符串</param>
        /// <param name="customSpecialChars">自定义特殊字符集（null 则使用默认）</param>
        /// <returns>true=以特殊字符开头，false=否（含空字符串、仅字母数字开头）</returns>
        public static bool StartsWithSpecialChar(string input, string customSpecialChars = null)
        {
            // 1. 空值/空字符串直接返回 false
            if (string.IsNullOrEmpty(input))
            {
                LogW("StartsWithSpecialChar: 输入字符串为空");
                return false;
            }

            // 2. 确定特殊字符集（优先使用自定义，无则用默认）
            string specialChars = customSpecialChars ?? DEFAULT_SPECIAL_CHARS;

            // 3. 正则表达式：匹配字符串开头是否为特殊字符集中的任意一个
            // 注：特殊字符集中的 ] ^ - 需转义，但已包含在 DEFAULT_SPECIAL_CHARS 中，无需额外处理
            string regexPattern = $"^[{Regex.Escape(specialChars)}]";
            return Regex.IsMatch(input, regexPattern);
        }
        void Dispose()
        {}
        
        /// <summary>
        /// 读取所有Excel文件
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="workSheetMap"></param>
        public async void ReadExcel()
        {
            if (package != null)
            {
                package.Dispose();
                package = null;
            }
            if (string.IsNullOrEmpty(directoryPath))
            {
                LogE($"DirectoryPath is null or empty");
                return;
            }
            if (workSheetMap == null)
            {
                LogE($"WorkSheetMap is null");
                return;
            }

            ExcelPackage.License.SetNonCommercialPersonal("ExcelReader");
            if (!Directory.Exists(directoryPath))
            {
                LogW($"Can`t find directory:{directoryPath}");
                return;
            }

            var files = Directory.GetFiles(directoryPath, "*.xlsx", SearchOption.AllDirectories);
            foreach (var filePath in files)
            {
                if (!File.Exists(filePath))
                {
                    LogW($"Can`t find file:{filePath}");
                    continue;
                }
                LogI($"<----------Read file--------->\n{filePath}");
                package = new ExcelPackage(filePath);
                if (package.Workbook == null || package.Workbook.Worksheets == null)
                {
                    LogE($"Can`t read excel,file path:{filePath}");
                    continue;
                }
                foreach (var sheet in package.Workbook.Worksheets)
                {
                    if(sheet == null)
                        continue;
                    var sheetName = sheet.Name;
                    if (!Regex.IsMatch(sheetName, regexValue))
                    {
                        LogI($"Regex error:[{sheetName}] ====> [{regexValue}]");
                        continue;
                    }
                    if (!workSheetMap.TryAdd(sheetName, sheet))
                    {
                        LogW($"Have same sheet name: {sheetName}");
                    }
                }
            }
        }
        
        public static string SelectDirectory()
        {
#if UNITY_EDITOR

            var path = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "");
            string rootPath = Application.dataPath.Replace("/Assets", "");
            if (path.StartsWith(rootPath))
            {
                path = path.Replace(rootPath, ".");
            }
            path = path.Replace("/", @"\");
            return path;
#endif
            return Application.dataPath;
        }


        static void LogE(string message)
        {
            Debug.LogError("[<color=#66ccff>ExcelReader</color>] "+message);
        }
        static void LogI(string message)
        {
            Debug.Log("[<color=#66ccff>ExcelReader</color>] "+message);
        }
        static void LogW(string message)
        {
            Debug.LogWarning("[<color=#66ccff>ExcelReader</color>] "+message);
        }
    }
}

