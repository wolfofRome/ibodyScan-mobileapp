using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace FitAndShape
{
    public class BootLifetimeScope : LifetimeScope
    {
        [SerializeField] BootView _bootView;
        [SerializeField] WebGroupView _webGroupView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_bootView);
            builder.RegisterComponent(_webGroupView);
            builder.RegisterEntryPoint<BootPresenter>(Lifetime.Singleton);
        }
    }
}