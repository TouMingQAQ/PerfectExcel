using System;
using System.Collections.Generic;
using UnityEngine;

namespace PerfectExcel.Convert
{
    public interface IStringConvert
    {
        public object Convert(string value,char[] split = null);
    }
    public class ToStr : IStringConvert
    {
        public object Convert(string value,char[] split = null)
        {
            return value;
        }
    }

    public class ToFloat : IStringConvert
    {
        public object Convert(string value,char[] split = null)
        {
            if (float.TryParse(value, out var res))
                return res;
            return 0;
        }
    }
    public class ToDouble : IStringConvert
    {
        public object Convert(string value,char[] split = null)
        {
            if (double.TryParse(value, out var res))
                return res;
            return 0;
        }
    }
    public class ToUInt16 : IStringConvert
    {
        public object Convert(string value,char[] split = null)
        {
            if (ushort.TryParse(value, out var res))
                return res;
            return 0;
        }
    }
    public class ToUInt32 : IStringConvert
    {
        public object Convert(string value,char[] split = null)
        {
            if (uint.TryParse(value, out var res))
                return res;
            return 0;
        }
    }
    public class ToUInt64 : IStringConvert
    {
        public object Convert(string value,char[] split = null)
        {
            if (ulong.TryParse(value, out var res))
                return res;
            return 0;
        }
    }
    public class ToInt16 : IStringConvert
    {
        public object Convert(string value,char[] split = null)
        {
            if (short.TryParse(value, out var res))
                return res;
            return 0;
        }
    }
    public class ToInt32 : IStringConvert
    {
        public object Convert(string value,char[] split = null)
        {
            if (int.TryParse(value, out var res))
                return res;
            return 0;
        }
    }

    public class ToInt64 : IStringConvert
    {
        public object Convert(string value,char[] split = null)
        {
            if (long.TryParse(value, out var res))
                return res;
            return 0;
        }
    }
    public class ToBool : IStringConvert
    {
        public object Convert(string value,char[] split = null)
        {
            if (bool.TryParse(value, out var res))
                return res;
            return false;
        }
    }

    public class ToEnum<T> : IStringConvert where T : struct
    {
        public object Convert(string value,char[] split = null)
        {
            if (Enum.TryParse(value, out T res))
                return res;
            return default(T);
        }
    }

    public class ToInt16Array : IStringConvert
    {
        public object Convert(string value, char[] split = null)
        {
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
        public object Convert(string value, char[] split = null)
        {
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
        public object Convert(string value, char[] split = null)
        {
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

