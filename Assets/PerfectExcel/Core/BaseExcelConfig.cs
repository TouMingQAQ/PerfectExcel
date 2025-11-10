using System;
using System.Collections.Generic;
using UnityEngine;

namespace PerfectExcel.Core
{
    public abstract class BaseExcelConfig<TKey,TValue> : ScriptableObject
    {
        public string workName;
        public List<TValue> valueList = new();
        public Dictionary<TKey, TValue> valueMap;

        protected abstract TKey GetKey(TValue value);

        protected virtual void OnRead(){}
    }
}

