using Amatib;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace FitAndShape
{
    public class FitAndShapeLifetimeScope : LifetimeScope
    {
        [SerializeField] FitAndShapeView _fitAndShapeView;
        [SerializeField] ArrowView _arrowView;
        [SerializeField] ModelView _modelView;
        [SerializeField] RenderTextureUpdater _renderTextureUpdater;
        [SerializeField] PosturePageFrameView _posturePageFrameView;
        [SerializeField] PostureDetailPageFrame _postureDetailPageFrame;
        [SerializeField] PostureSummaryView _postureSummaryView;
        [SerializeField] PostureDetailView _postureDetailView;
        [SerializeField] PostureArrowView _postureArrowView;
        [SerializeField] MeasurementView _measurementView;
        [SerializeField] WaistHistoryView _waistHistoryView;
        [SerializeField] PostureAdviceAsset _postureAdviceAsset;
        [SerializeField] ModelViewParameter _modelViewParameter;
        [SerializeField] LoadingView _loadingView;
        [SerializeField] HeaderView _headerView;
        [SerializeField] MorphView _morphView;
        [SerializeField] TabBarView _tabBarView;
        [SerializeField] OptionView _optionView;
        [SerializeField] FitAndShapeInfoView _fitAndShapeInfoView;
        [SerializeField] WebGroupView _webGroupView;
        [SerializeField] SelectItemGroupView _selectItemGroupView;
        [SerializeField] HeaderSelectGroupView _headerSelectGroupView;
        [SerializeField] SelectGroupView _selectGroupView;
        [SerializeField] NotFindView _notFindView;
        [SerializeField] AppParameter _appParameter;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_fitAndShapeView);
            builder.RegisterComponent(_arrowView);
            builder.RegisterComponent(_modelView);
            builder.RegisterComponent(_renderTextureUpdater);
            builder.RegisterComponent(_posturePageFrameView);
            builder.RegisterComponent(_postureDetailPageFrame);
            builder.RegisterComponent(_postureSummaryView);
            builder.RegisterComponent(_postureDetailView);
            builder.RegisterComponent(_postureArrowView);
            builder.RegisterComponent(_measurementView);
            builder.RegisterComponent(_waistHistoryView);
            builder.RegisterComponent(_postureAdviceAsset);
            builder.RegisterComponent(_modelViewParameter);
            builder.RegisterComponent(_loadingView);
            builder.RegisterComponent(_morphView);

#if UNITY_WEBGL_API
            builder.RegisterEntryPoint<FitAndShapePresenter>(Lifetime.Singleton);
            builder.RegisterComponent(_selectGroupView);
#else
            builder.RegisterEntryPoint<FitAndShapePresenterApp>(Lifetime.Singleton);
            builder.RegisterComponent(_headerView);
            builder.RegisterComponent(_tabBarView);
            builder.RegisterComponent(_optionView);
            builder.RegisterComponent(_fitAndShapeInfoView);
            builder.RegisterComponent(_webGroupView);
            builder.RegisterComponent(_selectItemGroupView);
            builder.RegisterComponent(_headerSelectGroupView);
            builder.RegisterComponent(_notFindView);
            builder.RegisterComponent(_appParameter);
#endif
        }
    }
}
