using Amatib.ObjViewer.Domain;

namespace FitAndShape
{
    public interface IFitAndShapeModelApp
    {
        FitAndShapeServiceType FitAndShapeServiceType { get; }
        FitAndShapeServiceType ExchangeMode();
    }

    public sealed class FitAndShapeModelApp : IFitAndShapeModelApp
    {
        public FitAndShapeServiceType FitAndShapeServiceType { get; private set; }

        public FitAndShapeModelApp(FitAndShapeServiceType fitAndShapeServiceType)
        {
            FitAndShapeServiceType = fitAndShapeServiceType;
        }

        public FitAndShapeServiceType ExchangeMode()
        {
            switch (FitAndShapeServiceType)
            {
                case FitAndShapeServiceType.Measuremenet:
                    FitAndShapeServiceType = FitAndShapeServiceType.Distortion;
                    break;
                case FitAndShapeServiceType.Distortion:
                    FitAndShapeServiceType = FitAndShapeServiceType.Measuremenet;
                    break;
            }
            return FitAndShapeServiceType;
        }
    }
}