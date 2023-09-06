using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UniRx;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

namespace FitAndShape
{
    public sealed class WebGroupView : MonoBehaviour
    {
        [SerializeField] List<WebViewManager> _webViewList;

        WebViewManager _currentWebView;

        public async UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            await UniTask.Delay(100, cancellationToken: cancellationToken);

            Initialize();

            Reset(cancellationToken);
        }

        void Initialize()
        {
            foreach (var item in _webViewList)
            {
                item.Initialize();
            }
        }

        public async UniTask ShowAsync(ScreenType screenType, SiteType siteType, CancellationToken cancellationToken)
        {
            var item = _webViewList.Where(n => n.ScreenType == screenType).FirstOrDefault();

            if (item == null)
            {
                _currentWebView = null;
                return;
            }

            _currentWebView = item;

            await _currentWebView.LoadWebViewAsync(siteType, cancellationToken);
        }

        public async UniTask HideAsync(CancellationToken cancellationToken)
        {
            if (_currentWebView == null)
            {
                return;
            }

            await _currentWebView.HideWebViewAsync(UniWebViewTransitionEdge.Left, cancellationToken);
        }

        public void HideAll(CancellationToken cancellationToken)
        {
            foreach (var item in _webViewList)
            {
                item.HideWebView(UniWebViewTransitionEdge.Left, cancellationToken);
            }
        }

        public void Reset(CancellationToken cancellationToken)
        {
            foreach (var item in _webViewList)
            {
                item.Reset(cancellationToken);
            }
        }

        public void SetLoginCookies(LoginData loginData)
        {
            foreach (var item in _webViewList)
            {
                item.SetLoginCookies(loginData);
            }
        }

        public void SetPageFinishedEvent(UnityAction<string> action)
        {
            foreach (var item in _webViewList)
            {
                item.OnPageFinished.Subscribe(n => action(n)).AddTo(this);
            }
        }

        public void SetHideEvent(UnityAction<Unit> action)
        {
            foreach (var item in _webViewList)
            {
                item.OnHide.Subscribe(n => action(n)).AddTo(this);
            }
        }

        public void SetMessageReceivedEvent(UnityAction<string> action)
        {
            foreach (var item in _webViewList)
            {
                item.OnMessageReceived.Subscribe(n => action(n)).AddTo(this);
            }
        }

        public void SetExchangeCreatedAtEvent(UnityAction<string> action)
        {
            foreach (var item in _webViewList)
            {
                item.OnExchangeCreatedAt.Subscribe(n => action(n)).AddTo(this);
            }
        }

        public void SetExchangePlaceEvent(UnityAction<string> action)
        {
            foreach (var item in _webViewList)
            {
                item.OnExchangePlace.Subscribe(n => action(n)).AddTo(this);
            }
        }

        public bool CanGoBack()
        {
            if (_currentWebView == null)
            {
                return false;
            }

            return _currentWebView.CanGoBack;
        }

        public void GoBack()
        {
            if (_currentWebView == null)
            {
                return;
            }

            _currentWebView.GoBack();
        }
    }
}