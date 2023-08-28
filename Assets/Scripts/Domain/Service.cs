using System;

namespace Amatib.ObjViewer.Domain
{
    public enum Service
    {
        AutoMeasure,
        AutoTailor,
        FitAndShape,
    }
    
    public static class ServiceExtensions
    {
        public static Service FromHost(string host)
        {
            if (host.Contains(Service.AutoMeasure.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return Service.AutoMeasure;
            }

            if (host.Contains(Service.FitAndShape.ToString().Replace("And", "-"), StringComparison.OrdinalIgnoreCase))
            {
                return Service.FitAndShape;
            }

            return Service.AutoTailor;
        }
    }
}