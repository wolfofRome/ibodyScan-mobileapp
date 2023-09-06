using System;
using System.Threading;
using Amatib.ObjViewer.Domain;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace FitAndShape
{
    public sealed class BootPresenter : IInitializable
    {
        [Inject] readonly Parameter _parameter;
        [Inject] readonly BootView _bootView;
        [Inject] readonly WebGroupView _webGroupView;

        async void IInitializable.Initialize()
        {
#if UNITY_ANDROID || UNITY_IOS
            try
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                await _webGroupView.InitializeAsync(cancellationToken);

                await AuthManager.Instance.AutoLogin(cancellationToken);

                _bootView.LoadAppScene();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);

                _bootView.LoadLoginScene();
            }

            return;
#else
            switch (_parameter.FitAndShapeServiceType)
            {
                case FitAndShapeServiceType.None:
                    _bootView.LoadAutotailorScene();
                    break;
                default:
                    _bootView.LoadFitAndShapeScene();
                    break;
            }
#endif
        }
    }
}