using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace FitAndShape
{
    public class LoginLifetimeScope : LifetimeScope
    {
        [SerializeField] LoadingView _loadingView;
        [SerializeField] HeaderView _headerView;
        [SerializeField] LoginView _loginView;
        [SerializeField] WebGroupView _webGroupView;
        [SerializeField] ServiceTypeGroupView _serviceTypeGroupView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<LoginPresenter>(Lifetime.Singleton);
            builder.RegisterComponent(_loadingView);
            builder.RegisterComponent(_headerView);
            builder.RegisterComponent(_loginView);
            builder.RegisterComponent(_webGroupView);
            builder.RegisterComponent(_serviceTypeGroupView);
        }
    }
}
