using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace FitAndShape
{
    [Serializable]
    public sealed class WaistHistoryEntity
    {
        [SerializeField] int current_page;
        [SerializeField] List<WasitHistoryValue> data;
        [SerializeField] int last_page;

        public int CurrentPage => current_page;
        public List<WasitHistoryValue> Data => data;
        public int LastPage => last_page;
    }

    [Serializable]
    public sealed class WasitHistoryValue
    {
        [SerializeField] int waist;
        [SerializeField] string created_at;

        public int Waist => waist;
        public string CreatedAt
        {
            get
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(created_at, null, DateTimeStyles.AssumeUniversal);
                return dateTimeOffset.ToString("yyyy年MM月dd日(ddd) HH:mm:ss");
            }
        }
    }
}