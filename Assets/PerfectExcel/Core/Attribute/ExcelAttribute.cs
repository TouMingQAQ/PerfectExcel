using System;
using UnityEngine;

namespace PerfectExcel.Attribute
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ExcelAttribute : System.Attribute
    {
        public Type convertType;
        public ExcelAttribute()
        {
            this.convertType = null;
        }
        public ExcelAttribute(Type convertType)
        {
            this.convertType = convertType;
        }
        
    }
    public interface IExcelConvertValue
    {
        
    }
    [AttributeUsage(AttributeTargets.Field)]
    public class ExcelSplitAttribute : System.Attribute,IExcelConvertValue
    {
        public char[] split;

        public ExcelSplitAttribute(char[] split)
        {
            this.split = split;
        }
    }
    [AttributeUsage(AttributeTargets.Field)]
    public class ExcelBoolAttribute : System.Attribute,IExcelConvertValue
    {
        public string TrueStr;
        public string FalseStr;

        public ExcelBoolAttribute(string trueStr, string falseStr)
        {
            this.TrueStr = trueStr;
            this.FalseStr = falseStr;
        }
    }

 
}

