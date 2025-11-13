using System;
using System.Collections.Generic;
using PerfectExcel.Attribute;
using UnityEngine;

namespace PerfectExcel.Convert
{
    public interface IStringConvert
    {
        public object Convert(string value,IExcelConvertValue convertValue = null);
    }


    
    public class ToStr : IStringConvert
    {
        public object Convert(string value,IExcelConvertValue convertValue = null)
        {
            return value;
        }
    }

    public class ToFloat : IStringConvert
    {
        public object Convert(string value,IExcelConvertValue convertValue = null)
        {
            if (float.TryParse(value, out var res))
                return res;
            return 0;
        }
    }
    public class ToDouble : IStringConvert
    {
        public object Convert(string value,IExcelConvertValue convertValue = null)
        {
            if (double.TryParse(value, out var res))
                return res;
            return 0;
        }
    }
    public class ToUInt16 : IStringConvert
    {
        public object Convert(string value,IExcelConvertValue convertValue = null)
        {
            if (ushort.TryParse(value, out var res))
                return res;
            return 0;
        }
    }
    public class ToUInt32 : IStringConvert
    {
        public object Convert(string value,IExcelConvertValue convertValue = null)
        {
            if (uint.TryParse(value, out var res))
                return res;
            return 0;
        }
    }
    public class ToUInt64 : IStringConvert
    {
        public object Convert(string value,IExcelConvertValue convertValue = null)
        {
            if (ulong.TryParse(value, out var res))
                return res;
            return 0;
        }
    }
    public class ToInt16 : IStringConvert
    {
        public object Convert(string value,IExcelConvertValue convertValue = null)
        {
            if (short.TryParse(value, out var res))
                return res;
            return 0;
        }
    }
    public class ToInt32 : IStringConvert
    {
        public object Convert(string value,IExcelConvertValue convertValue = null)
        {
            if (int.TryParse(value, out var res))
                return res;
            return 0;
        }
    }

    public class ToInt64 : IStringConvert
    {
        public object Convert(string value,IExcelConvertValue convertValue = null)
        {
            if (long.TryParse(value, out var res))
                return res;
            return 0;
        }
    }
    public class ToBool : IStringConvert
    {
        public object Convert(string value,IExcelConvertValue convertValue = null)
        {
            if (convertValue != null && convertValue is ExcelBoolAttribute boolConvertValue)
            {
                if (value.Equals(boolConvertValue.TrueStr))
                    return true;
                else if (value.Equals(boolConvertValue.FalseStr))
                    return false;
            }
            if (bool.TryParse(value, out var res))
                return res;
            return false;
        }
    }

    public class ToEnum<T> : IStringConvert where T : struct
    {
        public object Convert(string value,IExcelConvertValue convertValue = null)
        {
            if (Enum.TryParse(value, out T res))
                return res;
            return default(T);
        }
    }

    public class ToInt16Array : IStringConvert
    {
        public object Convert(string value, IExcelConvertValue convertValue = null)
        {
            if (convertValue == null || convertValue is not ExcelSplitAttribute chars)
                return null;
            var split = chars.split;
            if (split == null || split.Length < 1)
                return null;
            var splitChar = split[0];
            var strs = value.Split(splitChar);
            List<short> values = new();
            for (int i = 0,count = strs.Length; i < count; i++)
            {
                values.Add(short.TryParse(strs[i], out short v) ? v : (short)0);
            }
            return values;
        }
    }
    public class ToInt32Array : IStringConvert
    {
        public object Convert(string value, IExcelConvertValue convertValue = null)
        {
            if (convertValue == null || convertValue is not ExcelSplitAttribute chars)
                return null;
            var split = chars.split;
            if (split == null || split.Length < 1)
                return null;
            var splitChar = split[0];
            var strs = value.Split(splitChar);
            List<int> values = new();
            for (int i = 0,count = strs.Length; i < count; i++)
            {
                values.Add(int.TryParse(strs[i], out int v) ? v : (int)0);
            }
            return values;
        }
    }
    public class ToInt64Array : IStringConvert
    {
        public object Convert(string value, IExcelConvertValue convertValue = null)
        {
            if (convertValue == null || convertValue is not ExcelSplitAttribute chars)
                return null;
            var split = chars.split;
            if (split == null || split.Length < 1)
                return null;
            var splitChar = split[0];
            var strs = value.Split(splitChar);
            List<long> values = new();
            for (int i = 0,count = strs.Length; i < count; i++)
            {
                values.Add(long.TryParse(strs[i], out long v) ? v : (long)0);
            }
            return values;
        }
    }
}

