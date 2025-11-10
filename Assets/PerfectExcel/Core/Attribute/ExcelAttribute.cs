using System;
using UnityEngine;

namespace PerfectExcel.Attribute
{
    public class ExcelAttribute : System.Attribute
    {
        public static char[] Split = new[] { ':' ,';', '|'};
        public Type convertType;
        public char[] splitChars;
        public ExcelAttribute()
        {
            this.convertType = null;
            splitChars = null;
        }
        public ExcelAttribute(Type convertType,char[] splitChars = null)
        {
            this.convertType = convertType;
            this.splitChars = splitChars;
            this.splitChars ??= Split;
        }
    }
}

