using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UnityEngine.SceneManagement;

namespace FitAndShape
{
    public sealed class LoginPresenter : IInitializable
    {
        public static readonly string SceneName = "Login";

        [Inject] readonly LoadingView _loadingView;
        [Inject] readonly HeaderView _headerView;
        [Inject] readonly LoginView _loginView;
        [Inject] readonly WebGroupView _webGroupView;
        [Inject] readonly ServiceTypeGroupView _serviceTypeGroupView;

        CancellationToken _cancellationToken;
        
        async void IInitializable.Initialize()
        {
            Initialize();

            _loadingView.Visible = true;

            await _webGroupView.InitializeAsync(_cancellationToken);

            RegisterEvent();

            _loadingView.Visible = false;

            if (LoginInfo.Exist())
            {
                await LoginAsync(LoginType.Auto);

                return;
            }

            _loginView.Show();
        }

        void Initialize()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = cancellationTokenSource.Token;

            _loginView.Initialize();

            _headerView.Initialize();

            _serviceTypeGroupView.Initialize();
        }

        void RegisterEvent()
        {
            _loginView.OnLoginButtonClick.Subscribe(_ =>
            {
                LoginAsync(LoginType.None).Forget();

            }).AddTo(_loginView);

            _loginView.OnDemoButtonClick.Subscribe(_ =>
            {
                LoginAsync(LoginType.Demo).Forget();

            }).AddTo(_loginView);

            _loginView.OnNewIDRegisterLinkButtonClick.Subscribe(async _ =>
            {
                _loadingView.Visible = true;

                await _webGroupView.ShowAsync(ScreenType.Register, SiteType.Register, _cancellationToken);

                _loadingView.Visible = false;

            }).AddTo(_loginView);

            _loginView.OnPasswordReregisterLinkButtonClick.Subscribe(async _ =>
            {
                _loadingView.Visible = true;

                await _webGroupView.ShowAsync(ScreenType.Password, SiteType.Password, _cancellationToken);

                _loadingView.Visible = false;

            }).AddTo(_loginView);


            _webGroupView.SetPageFinishedEvent((n) =>
            {
                _headerView.ShowPreve(n);
            });

            _webGroupView.SetHideEvent(async (n) =>
            {
                _loadingView.Visible = true;

                _headerView.HidePreve();

                await _webGroupView.HideAsync(_cancellationToken);

                _loadingView.Visible = false;
            });

            _webGroupView.SetMessageReceivedEvent(async (n) =>
            {
                _loadingView.Visible = true;

                await _webGroupView.HideAsync(_cancellationToken);

                _loadingView.Visible = false;
            });


            _headerView.OnPreveClick.Subscribe(async _ =>
            {
                if (_webGroupView.CanGoBack())
                {
                    _webGroupView.GoBack();
                    return;
                }

                _loadingView.Visible = true;

                _headerView.HidePreve();

                await _webGroupView.HideAsync(_cancellationToken);

                _loadingView.Visible = false;

            }).AddTo(_loginView);
        }

        async UniTask LoginAsync(LoginType loginType)
        {
            try
            {
                _loadingView.Visible = true;

                ILoginModel loginModel = new LoginModel(_webGroupView.GetUserAgent());

                LoginResponse loginResponse;

                LoginInfo loginInfo = PlayerPrefsUtils.GetObject<LoginInfo>(LoginInfo.Key);

                switch (loginType)
                {
                    case LoginType.Demo:
                        loginInfo = new LoginInfo() { UserId = string.Empty, Password = string.Empty };
                        loginResponse = await loginModel.Login(loginInfo.UserId, loginInfo.Password, _cancellationToken);
                        break;
                    case LoginType.Auto:
                        loginResponse = await loginModel.Login(loginInfo.UserId, loginInfo.Password, _cancellationToken);
                        break;
                    default:
                        loginResponse = await loginModel.Login(_loginView.UserID, _loginView.Password, _cancellationToken);
                        loginInfo = new LoginInfo() { UserId = _loginView.UserID, Password = _loginView.Password };
                        break;
                }

                _loginView.ErrorMessage = string.Empty;

                loginInfo.FitAndShapeServiceType = _serviceTypeGroupView.FitAndShapeServiceType;

                PlayerPrefsUtils.SetObject(LoginInfo.Key, loginInfo);
                PlayerPrefsUtils.SetObject(LoginData.Key, loginResponse.Data);

                SceneManager.LoadScene(FitAndShapePresenterApp.SceneName);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                _loginView.ErrorMessage = "IDまたはパスワードに誤りがあります";
                LoginInfo.Clear();
                LoginData.Clear();
            }
            finally
            {
                _loadingView.Visible = false;
            }
        }
    }
}