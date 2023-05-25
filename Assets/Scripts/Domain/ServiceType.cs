using System;

namespace Amatib.ObjViewer.Domain
{
    public enum FitAndShapeServiceType
    {
        None,
        /// <summary>
        /// ゆがみFinder
        /// </summary>
        Distortion,
        /// <summary>
        /// 3D身体測定
        /// </summary>
        Measuremenet,
    }
    
    public static class FitAndShapeServiceTypeExtensions
    {
        public static FitAndShapeServiceType GetType(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return FitAndShapeServiceType.None;
            }

            if (value.Contains("body-distortion", StringComparison.OrdinalIgnoreCase))
            {
                return FitAndShapeServiceType.Distortion;
            }

            if (value.Contains("body-measurement", StringComparison.OrdinalIgnoreCase))
            {
                return FitAndShapeServiceType.Measuremenet;
            }

            return FitAndShapeServiceType.None;
        }
    }
}