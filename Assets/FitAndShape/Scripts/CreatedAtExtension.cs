using System;

namespace FitAndShape
{
    public sealed class CreatedAtExtension
    {
        public static string Format(string created_at)
        {
            DateTimeOffset dateTimeOffset;
            if (DateTimeOffset.TryParse(created_at, out dateTimeOffset))
            {
                return dateTimeOffset.ToString("yyyy年MM月dd日(ddd) HH:mm:ss");
            }
            else
            {
                return string.Empty;
            }
        }
    }
}