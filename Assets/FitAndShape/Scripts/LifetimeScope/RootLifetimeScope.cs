using Amatib.ObjViewer.Domain;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace FitAndShape
{
    public class RootLifetimeScope : LifetimeScope
    {
        [SerializeField] FitAndShapeParameter _fitAndShapeParameter;
        [SerializeField] FitAndShapeParameter _fitAndShapeParameterApp;
        [SerializeField] PostureWarningPivotAsset _postureWarningPivotAsset;
        [SerializeField] PostureWarningPivotAsset _postureWarningPivotAssetApp;

        protected override void Configure(IContainerBuilder builder)
        {
#if !UNITY_EDITOR
            Debug.unityLogger.logEnabled = false; // ログを無効化する    
#endif

            FitAndShapeParameter fitAndShapeParameter = null;

#if UNITY_WEBGL_API
            fitAndShapeParameter = _fitAndShapeParameter;
            builder.RegisterComponent(_postureWarningPivotAsset);
#else
            fitAndShapeParameter = _fitAndShapeParameterApp;
            builder.RegisterComponent(_postureWarningPivotAssetApp);
#endif
            builder.RegisterComponent(fitAndShapeParameter);


#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            string url = fitAndShapeParameter.TestUrl;
#else
            string url = Application.absoluteURL;    
#endif

            Parameter parameter = new Parameter(url);
            builder.RegisterInstance<Parameter>(parameter);
        }
    }
}