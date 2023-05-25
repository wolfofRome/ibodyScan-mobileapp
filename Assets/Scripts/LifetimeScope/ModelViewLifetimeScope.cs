using VContainer;
using VContainer.Unity;
using UnityEngine;
using Amatib.ObjViewer.Presentation;

namespace Amatib.ObjViewer
{
    public class ModelViewLifetimeScope : LifetimeScope
    {
        [SerializeField] ModelViewManager _modelViewManager;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_modelViewManager);
        }
    }
}
