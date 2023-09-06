using System;
using System.Text;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace FitAndShape
{
    public enum SiteType
    {
        None,
        Register,
        Password,
        Home,
        ProductSearch,
        MeasurementData,
        MyPage,
        ScanMeasurement,
        ScanDistortion,
        ProfileMeasurement,
        ProfileDistortion,
        About,
        ShopList,
        News,
        PrivacyPolicy,
        PrivacyPolicyMeasurement,
        PrivacyPolicyDistortion,
        Corporation,
        Login,
        Complete,
    }

    public enum ScreenType
    {
        None,
        Register,
        Password,
        Scan,
        Profile,
        PrivacyPolicy,
    }

    public sealed class WebViewManager : MonoBehaviour
    {
        [SerializeField] ScreenType _screenType;
        [SerializeField] UniWebView _uniWebView;

        readonly static string BaseURL = "https://fit-shape.jp/user";
        readonly static string CookieURL = "https://fit-shape.jp";
        readonly static float TransitionDuration = 0.35f;

        public bool CanGoBack => _uniWebView != null && _uniWebView.CanGoBack;

        public IObservable<string> OnPageFinished => _onPageFinished;
        Subject<string> _onPageFinished = new Subject<string>();

        public IObservable<string> OnMessageReceived => _onMessageReceived;
        Subject<string> _onMessageReceived = new Subject<string>();

        public IObservable<string> OnExchangeCreatedAt => _onExchangeCreatedAt;
        Subject<string> _onExchangeCreatedAt = new Subject<string>();

        public IObservable<string> OnExchangePlace => _onExchangePlace;
        Subject<string> _onExchangePlace = new Subject<string>();

        public IObservable<Unit> OnHide => _onHide;
        Subject<Unit> _onHide = new Subject<Unit>();

        string _loadingUrl;
        bool _isInitialize = false;
        string _userAgent = string.Empty;
        SiteType _siteType;
        bool _isHide = false;

        public ScreenType ScreenType => _screenType;

        public void SetLoginCookies(LoginData loginData)
        {
            RemoveCookies();

            SetCookie("fitandshape_user_session", loginData.UserSession);
            SetCookie("XSRF-TOKEN", loginData.UserSession);
            SetCookie("is-login", "true");
        }

        void SetCookie(string key, string value)
        {
            var cookie = key + "=" + value;
            UniWebView.SetCookie(CookieURL, cookie);
        }

        public void RemoveCookies()
        {
            UniWebView.RemoveCookies(CookieURL);
        } 

        public void Initialize()
        {
            if (_isInitialize)
            {
                return;
            }

            _isInitialize = true;

            _siteType = SiteType.None;

            _loadingUrl = GetSiteUrl(SiteType.Home);

            _uniWebView.SetUserAgent(AuthManager.Instance.GetUserAgent());
            _uniWebView.AddUrlScheme(FSConstants.WEBVIEW_URL_SCHEME);
            _uniWebView.SetHorizontalScrollBarEnabled(false);
            _uniWebView.SetAllowHTTPAuthPopUpWindow(false);
            _uniWebView.SetShowSpinnerWhileLoading(true);

            SetSpinnerText();

            _uniWebView.OnPageStarted += (webView, url) =>
            {
                //Debug.Log("OnPageStarted - url = " + url);

                _loadingUrl = url;
            };

            _uniWebView.OnPageFinished += (webView, statusCode, url) =>
            {
                //Debug.Log("OnPageFinished - url = " + url);
                //Debug.Log("OnPageFinished - statusCode = " + statusCode);
                //Debug.Log("OnPageFinished - siteType = " + _siteType);

                if (statusCode != 200) return;

                if (GetSiteUrl(SiteType.Login) == url || GetSiteUrl(SiteType.Complete) == url)
                {
                    _uniWebView.Stop();
                    _onHide.OnNext(Unit.Default);
                    return;
                }

                switch (_siteType)
                {
                    case SiteType.ScanDistortion:
                    case SiteType.ScanMeasurement:
                        break;
                    case SiteType.ProfileDistortion:
                    case SiteType.ProfileMeasurement:

                        if (GetSiteUrl(SiteType.ProfileDistortion) == url || GetSiteUrl(SiteType.ProfileMeasurement) == url)
                        {
                            return;
                        }

                        if (!gameObject.activeSelf)
                        {
                            return;
                        }

                        _onPageFinished.OnNext("戻る");
                        break;
                    default:
                        _onPageFinished.OnNext("戻る");
                        return;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("const links = document.querySelectorAll('.app-3dModel-btn');");
                sb.AppendLine("links.forEach(link => {");
                sb.AppendLine("  link.href = '" + AuthManager.Instance.GetUserAgent() + "://3dmodel_'.concat(link.getAttribute('data-measurement_number')) + '_' + link.getAttribute('data-created_at') + '_' + link.getAttribute('data-place');");
                sb.AppendLine("});");
                webView.AddJavaScript(sb.ToString());
            };

            _uniWebView.OnMessageReceived += (webView, message) =>
            {
                switch (_siteType)
                {
                    case SiteType.ScanDistortion:
                    case SiteType.ScanMeasurement:
                        string[] array = message.Path.Split("_");
                        _onMessageReceived.OnNext(array[1]);
                        _onExchangeCreatedAt.OnNext(array[2].Replace(' ', '+'));
                        _onExchangePlace.OnNext(array[3]);
                        break;
                }
            };
        }

        void SetSpinnerText()
        {
            if (_uniWebView == null)
            {
                return;
            }

            _uniWebView.SetSpinnerText("Loading");
        }

        public void LoadWebView(SiteType siteType)
        {
            _siteType = siteType;

            LoadWebView(GetSiteUrl(siteType), UniWebViewTransitionEdge.Left);
        }

        void LoadWebView(string url, UniWebViewTransitionEdge transitionEdge)
        {
            Debug.Log("url = " + url);

            gameObject.SetActive(true);

            _uniWebView.Load(url);
            _uniWebView.Show(true, transitionEdge, TransitionDuration);
        }

        public async UniTask LoadWebViewAsync(SiteType siteType, CancellationToken cancellationToken)
        {
            _siteType = siteType;

            await LoadWebViewAsync(GetSiteUrl(siteType), UniWebViewTransitionEdge.Left, cancellationToken);
        }

        async UniTask LoadWebViewAsync(string url, UniWebViewTransitionEdge transitionEdge, CancellationToken cancellationToken)
        {
            //Debug.Log("url = " + url);

            gameObject.SetActive(true);

            _uniWebView.Load(url);
            _uniWebView.Show(true, transitionEdge, TransitionDuration);

            await UniTask.Delay((int)TimeSpan.FromSeconds(TransitionDuration).TotalMilliseconds, cancellationToken: cancellationToken);
        }

        //public void ReloadWebView(UniWebViewTransitionEdge transitionEdge)
        //{
        //    LoadWebView(_loadingUrl, transitionEdge);
        //}

        public void Reset(CancellationToken cancellationToken)
        {
            _siteType = SiteType.None;

            RemoveCookies();

            CleanCache();

            SetSpinnerText();
        }

        async UniTask AllGoBackAsync(CancellationToken cancellationToken)
        {
            while (_uniWebView.CanGoBack)
            {
                _uniWebView.GoBack();

                if (!_uniWebView.CanGoBack)
                {
                    break;
                }

                await UniTask.Delay(100, cancellationToken: cancellationToken);
            }
        }

        public void GoBack()
        {
            if (_uniWebView == null)
            {
                return;
            }

            _uniWebView.GoBack();
        }

        public void CleanCache()
        {
            if (_uniWebView == null)
            {
                return;
            }

            _uniWebView.CleanCache();
        }

        public void HideWebView(UniWebViewTransitionEdge transitionEdge, CancellationToken cancellationToken)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (_isHide)
            {
                return;
            }

            _isHide = true;

            _uniWebView.Hide(true, transitionEdge, TransitionDuration, () =>
            {
                AllGoBackAsync(cancellationToken).Forget();

                Hide();

                _isHide = false;
            });
        }

        public async UniTask HideWebViewAsync(UniWebViewTransitionEdge transitionEdge, CancellationToken cancellationToken)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (_isHide)
            {
                return;
            }

            _isHide = true;

            _uniWebView.Hide(true, transitionEdge, TransitionDuration, () =>
            {
                AllGoBackAsync(cancellationToken).Forget();

                Hide();

                _isHide = false;
            });

            await UniTask.Delay((int)TimeSpan.FromSeconds(TransitionDuration).TotalMilliseconds, cancellationToken: cancellationToken);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public string GetSiteUrl(SiteType siteType)
        {
            return siteType switch
            {
                SiteType.Register => BaseURL + "/register/mobile-number/",
                SiteType.Password => BaseURL + "/password/mobile-number/",
                SiteType.Home => BaseURL + "/",
                SiteType.ProductSearch => BaseURL + "/products/",
                SiteType.MeasurementData => BaseURL + "/user/measurements/",
                SiteType.MyPage => BaseURL + "/user/mypage/",
                SiteType.ScanMeasurement => BaseURL + "/body-measurement/measurements/",
                SiteType.ScanDistortion => BaseURL + "/body-distortion/measurements/",
                SiteType.ProfileMeasurement => BaseURL + "/body-measurement/profile/show/",
                SiteType.ProfileDistortion => BaseURL + "/body-distortion/profile/show/",
                SiteType.About => BaseURL + "/about/",
                SiteType.ShopList => BaseURL + "/shoplist/",
                SiteType.News => BaseURL + "/news/",
                SiteType.PrivacyPolicy => BaseURL + "/register/privacy-policy/",
                SiteType.PrivacyPolicyMeasurement => BaseURL + "/body-measurement/privacy-policy/",
                SiteType.PrivacyPolicyDistortion => BaseURL + "/body-distortion/privacy-policy/",
                SiteType.Corporation => "https://www.i-body.co.jp",
                SiteType.Login => BaseURL + "/login/",
                SiteType.Complete => BaseURL + "/register/profile/complete/",
                _ => throw new ArgumentOutOfRangeException(nameof(siteType), siteType, null)
            };
        }
    }
}