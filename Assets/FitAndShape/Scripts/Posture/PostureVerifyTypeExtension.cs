using System.Collections.Generic;

namespace FitAndShape
{
    public static class PostureVerifyTypeExtension
    {
        private static readonly Dictionary<VerifyType, string>
            _measurementUnitMap = new Dictionary<VerifyType, string>(){
                { VerifyType.Length, "cm"},
                { VerifyType.Angle,  "度"}
            };

        public static string ToMeasurementUnit(this VerifyType type)
        {
            return _measurementUnitMap[type];
        }
    }
}