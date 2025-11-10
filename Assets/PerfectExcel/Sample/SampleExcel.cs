using System;
using System.Collections.Generic;
using PerfectExcel.Attribute;
using PerfectExcel.Convert;
using PerfectExcel.Core;
using UnityEngine;

namespace PerfectExcel.Sample
{
    [Serializable]
    public struct SampleValue
    {
        [Excel]
        public long ID;
        [Excel]
        public string StrValue;
        [Excel(typeof(ToInt32Array))]
        public List<int> IntArray;
    }
    [CreateAssetMenu(menuName = "Excel/Data/Sample",fileName = "SampleExcel")]
    public class SampleExcel : BaseExcelConfig<long,SampleValue>
    {
        protected override long GetKey(SampleValue value) => value.ID;
    }
}

