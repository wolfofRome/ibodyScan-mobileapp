using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Amatib.ObjViewer.Domain;
using Amatib.ObjViewer.Infrastructure;
using Amatib.ObjViewer.Presentation.Loaders;
using Codice.CM.Client.Differences.Merge;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace FitAndShape
{
    public sealed class FitAndShapePresenterApp : IInitializable
    {
        public static readonly string SceneName = "FitAndShapeApp";

        [Inject] readonly FitAndShapeView _fitAndShapeView;
        [Inject] readonly FitAndShapeParameter _fitAndShapeParameter;
        [Inject] readonly ArrowView _arrowView;
        [Inject] readonly ModelView _modelView;
        [Inject] readonly RenderTextureUpdater _renderTextureUpdater;
        [Inject] readonly PosturePageFrameView _posturePageFrameView;
        [Inject] readonly PostureDetailPageFrame _postureDetailPageFrame;
        [Inject] readonly PostureSummaryView _postureSummaryView;
        [Inject] readonly PostureDetailView _postureDetailView;
        [Inject] readonly PostureArrowView _postureArrowView;
        [Inject] readonly MeasurementView _measurementView;
        [Inject] readonly WaistHistoryView _waistHistoryView;
        [Inject] readonly PostureAdviceAsset _postureAdviceAsset;
        [Inject] readonly LoadingView _loadingView;
        [Inject] readonly HeaderView _headerView;
        [Inject] readonly TabBarView _tabBarView;
        [Inject] readonly OptionView _optionView;
        [Inject] readonly FitAndShapeInfoView _fitAndShapeInfoView;
        [Inject] readonly WebGroupView _webGroupView;
        [Inject] readonly SelectItemGroupView _selectItemGroupView;
        [Inject] readonly HeaderSelectGroupView _headerSelectGroupView;
        [Inject] readonly NotFindView _notFindView;
        [Inject] readonly AppParameter _appParameter;

        IAvatarModel _avatarModel;
        IPostureVerifyerModel _postureVerifyerModel;
        ICommentModel _commentModel;
        CancellationToken _cancellationToken;
        ApiClient _client;
        Vector3 _scale;
        string[] _objLines;
        IFitAndShapeModelApp _fitAndShapeModelApp;
        DisplayState _currentState = DisplayState.None;
        bool _isLogin = false;
        int currentColor = 0;

        bool IsDemo()
        {
            LoginData loginData = PlayerPrefsUtils.GetObject<LoginData>(LoginData.Key);
            return loginData.IsDemo;
        }

        async void IInitializable.Initialize()
        {
            Debug.Log("[FitAndShapePresenterApp Initialize]: start");
            _fitAndShapeView.CameraVisible = true;

            if (!LoginData.Exist())
            {
                SceneManager.LoadScene(LoginPresenter.SceneName);
                return;
            }

            Initialize();

            HideAll();

            _loadingView.Visible = true;

            await _webGroupView.InitializeAsync(_cancellationToken);

            _loadingView.Visible = false;

            _isLogin = await LoadSystemAsync(AuthManager.Instance.GetLoginInfo().FitAndShapeServiceType);

            SetLoginData();
            RegisterEvent();

            if (!_isLogin)
            {
                _notFindView.Show();
                return;
            }

            _notFindView.Hide();
            _fitAndShapeView.CameraVisible = false;
        }

        async UniTask LoadModelAsync(ColorType colorType, bool showLoading)
        {
            try
            {
                _modelView.OnReset();

                if (showLoading)
                {
                    _loadingView.Visible = true;
                }

                switch (colorType)
                {
                    case ColorType.Color:

                        if (!_modelView.IsLoadPointCloud)
                        {
                            _modelView.SetPointCloudMesh(PlyLoader.LoadAsMesh(await _client.Download("scan_data.ply"),
                                _scale));
                        }

                        _modelView.PointCloudModelVisible = true;
                        _modelView.MonochromeModelVisible = false;
                        break;

                    case ColorType.Monochrome:

                        if (!_modelView.IsLoadMonochrome)
                        {
                            _modelView.SetMonochromeMesh(ObjLoader.LoadAsMesh(_objLines, _scale));
                        }

                        _modelView.PointCloudModelVisible = false;
                        _modelView.MonochromeModelVisible = true;
                        break;
                }
            }
            catch (Exception e)
            {
                if (showLoading)
                {
                    Debug.LogException(e);
                    Clear();
                    SceneManager.LoadScene("LoadingError");
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (showLoading)
                {
                    _loadingView.Visible = false;
                }
            }
        }

        async UniTask LoadComment(ILoadCsvModel loadCsvModel, string measurementNumber)
        {
            string urlString =
                $"{_fitAndShapeParameter.CsvUrl}/measurements/{measurementNumber.Substring(2, measurementNumber.Length - 2)}/body-distortion-comments?key={_fitAndShapeParameter.ApiKey}";
            string commentData = await loadCsvModel.GetCsvData(urlString, _cancellationToken);
            _commentModel = new CommentModel(commentData);
        }

        void Initialize()
        {
            _scale = _fitAndShapeParameter.Scale;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = cancellationTokenSource.Token;

            _optionView.Initialize();

            _tabBarView.Initialize();

            _headerView.Initialize();

            _renderTextureUpdater.Initialize(_fitAndShapeParameter.FieldOfView);
            _renderTextureUpdater.SetDisplayCamera(_fitAndShapeView.DefaultAngle);
            _renderTextureUpdater.SetViewPortRect(_fitAndShapeParameter.ViewportRect);

            _postureDetailView.Initialize();

            _fitAndShapeInfoView.Initialize();

            _waistHistoryView.Initialize(_fitAndShapeParameter.CsvUrl, _fitAndShapeParameter.ApiKey);

            _headerSelectGroupView.Initialize();

            _selectItemGroupView.Initialize();
        }

        void RegisterEvent()
        {
            _posturePageFrameView.OnClickAsWarningButton.Subscribe(value =>
            {
                ShowPostureDetail(value.Number, value.Result);
            }).AddTo(_fitAndShapeView);

            _postureSummaryView.OnClick.Subscribe(value =>
            {
                _currentState = DisplayState.PostureDetail;

                ShowPostureDetail(value.Number, value.Result);
            }).AddTo(_fitAndShapeView);

            _measurementView.OnClick.Subscribe(x =>
            {
                _arrowView.Select(x);
                _arrowView.ArrowPartVisible = true;
                _modelView.OnReset();
            }).AddTo(_fitAndShapeView);

            _measurementView.OnHistoryClick.Subscribe(x =>
            {
                _arrowView.Select(x);
                _arrowView.ArrowPartVisible = true;
                _modelView.OnReset();

                switch (x)
                {
                    case MeasurementPart.WaistMaxCircumference:
                    case MeasurementPart.WaistMinCircumference:
                        _currentState = DisplayState.WaistHistory;
                        _headerView.ShowPreve(_waistHistoryView.GetTitle(x));
                        _headerView.ShowLogo(FitAndShapeServiceType.None);
                        _waistHistoryView.Show(x);
                        break;
                }
            }).AddTo(_fitAndShapeView);

            _fitAndShapeView.OnPlayerInput.Subscribe(_ => { _arrowView.ArrowPartVisible = false; })
                .AddTo(_fitAndShapeView);

            _headerView.OnExchangeModeClick.Subscribe(_ =>
            {
                HideSelectColorView();
                HideSelectAngleView();
                HideMeasurement();
                HidePostureSummary();

                _headerView.HidePreve();

                _modelView.OnReset();

                switch (_fitAndShapeModelApp.ExchangeMode())
                {
                    case FitAndShapeServiceType.Distortion:
                        ShowPostureSummary();
                        ShowSelectColorView();
                        ShowSelectAngleView();
                        break;
                    case FitAndShapeServiceType.Measuremenet:
                        _arrowView.Clear();
                        _arrowView.ArrowPartVisible = true;
                        _measurementView.Clear();
                        ShowMeasurement();
                        ShowSelectColorView();
                        break;
                }

                _headerView.ShowLogo(_fitAndShapeModelApp.FitAndShapeServiceType);
            }).AddTo(_fitAndShapeView);

            RegisterTabBarEvent();

            RegisterOptionEvent();

            _webGroupView.SetPageFinishedEvent((n) =>
            {
                //Debug.Log($"_webGroupView.SetPageFinishedEvent {n}, IsLogin:{IsLogin}");

                _headerView.ShowPreve(n);
                _headerView.ShowLogo(FitAndShapeServiceType.None);
            });

            _webGroupView.SetHideEvent(async (n) =>
            {
                _loadingView.Visible = true;

                _headerView.HidePreve();
                _headerView.ShowLogo(FitAndShapeServiceType.None);

                await _webGroupView.HideAsync(_cancellationToken);

                _loadingView.Visible = false;
            });

            _headerView.OnPreveClick.Subscribe(async _ =>
            {
                //Debug.Log($"HeaderView.OnPreveClick currentState:{_currentState}");

                switch (_currentState)
                {
                    case DisplayState.PostureDetail:
                        _currentState = DisplayState.PostureSummary;
                        _headerView.HidePreve();
                        ShowPostureSummary();
                        _headerView.ShowLogo(_fitAndShapeModelApp.FitAndShapeServiceType);
                        break;
                    case DisplayState.WaistHistory:
                        _currentState = DisplayState.Measurement;
                        _headerView.HidePreve();
                        _waistHistoryView.Hide();
                        _arrowView.ArrowPartVisible = true;
                        _modelView.OnReset();
                        _headerView.ShowLogo(_fitAndShapeModelApp.FitAndShapeServiceType);
                        break;
                    case DisplayState.Scan:
                        await ShowScan();
                        break;
                    default:

                        if (_webGroupView.CanGoBack())
                        {
                            _webGroupView.GoBack();
                            return;
                        }

                        _loadingView.Visible = true;

                        _headerView.HidePreve();

                        await _webGroupView.HideAsync(_cancellationToken);

                        if (_fitAndShapeModelApp != null)
                        {
                            _headerView.ShowLogo(_fitAndShapeModelApp.FitAndShapeServiceType);
                        }
                        else
                        {
                            _headerView.ShowLogo(FitAndShapeServiceType.None);
                        }

                        _loadingView.Visible = false;

                        break;
                }
            }).AddTo(_fitAndShapeView);

            _webGroupView.SetMessageReceivedEvent(async (n) =>
            {
                Debug.Log($"Select MeasurementNumber:{n}");

                _loadingView.Visible = true;

                await _webGroupView.HideAsync(_cancellationToken);

                _loadingView.Visible = false;

                _fitAndShapeView.CameraVisible = true;

                _modelView.Clear();

                HideAll();

                LoginData loginData = PlayerPrefsUtils.GetObject<LoginData>(LoginData.Key);
                loginData.SetMeasurementNumber(n);

                await LoadSystemAsync(_fitAndShapeModelApp.FitAndShapeServiceType);

                _headerView.ShowPreve("一覧へ戻る");
                _headerView.ShowLogo(FitAndShapeServiceType.None);

                _tabBarView.ShowButton(SelectButtonType.Scan);

                _currentState = DisplayState.Scan;

                _fitAndShapeView.CameraVisible = false;
            });

            _webGroupView.SetExchangeCreatedAtEvent(n =>
            {
                _fitAndShapeInfoView.DateText = CreatedAtExtension.Format(n);
            });

            _webGroupView.SetExchangePlaceEvent(n => { _fitAndShapeInfoView.PlaceText = HttpUtility.UrlDecode(n); });

            _headerSelectGroupView.OnButtonClick.Subscribe(n =>
            {
                _selectItemGroupView.Show(n.SelectType, n.SelectItemType);
            }).AddTo(_fitAndShapeView);

            _selectItemGroupView.OnButtonClick.Subscribe(async n => //Here!
            {
                _headerSelectGroupView.SetSelectItemType(n);

                switch (n.ToSelectType())
                {
                    case SelectType.Color:
                        ColorType colorType = n.ToColorType();
                        await LoadModelAsync(colorType, true);
                        break;
                    default:
                        _fitAndShapeView.DefaultAngle = n.ToAngle();
                        ShowPostureSummary();
                        break;
                }
            }).AddTo(_fitAndShapeView);
        }

        void RegisterTabBarEvent()
        {
            _tabBarView.OnMyPageButtonClick.Subscribe(async _ =>
            {
                // マイページクリック
                _loadingView.Visible = true;

                currentColor = GameObject.Find("Dropdown_Color").GetComponent<TMPro.TMP_Dropdown>().value;

                await _webGroupView.HideAsync(_cancellationToken);

                _loadingView.Visible = false;

                _modelView.Clear();

                HideAll();

                await LoadSystemAsync(_fitAndShapeModelApp.FitAndShapeServiceType);

                SetLoginData();
            }).AddTo(_fitAndShapeView);

            _tabBarView.OnProfileButtonClick.Subscribe(async _ =>
            {
                if (IsDemo())
                {
                    return;
                }

                _headerView.HidePreve();

                _headerView.HideExchangeMode();
                _optionView.Hide();

                _loadingView.Visible = true;

                await _webGroupView.HideAsync(_cancellationToken);

                SiteType siteType = SiteType.None;

                switch (_fitAndShapeModelApp.FitAndShapeServiceType)
                {
                    case FitAndShapeServiceType.Distortion:
                        siteType = SiteType.ProfileDistortion;
                        break;
                    case FitAndShapeServiceType.Measuremenet:
                        siteType = SiteType.ProfileMeasurement;
                        break;
                }

                await _webGroupView.ShowAsync(ScreenType.Profile, siteType, _cancellationToken);

                _loadingView.Visible = false;

                _currentState = DisplayState.Profile;

                _headerView.ShowPreve("戻る");
                _headerView.ShowLogo(FitAndShapeServiceType.None);
            }).AddTo(_fitAndShapeView);

            _tabBarView.OnScanButtonClick.Subscribe(async _ => { await ShowScan(); }).AddTo(_fitAndShapeView);

            _tabBarView.OnOptionButtonClick.Subscribe(async _ =>
            {
                _headerView.HidePreve();
                _headerView.HideExchangeMode();

                if (_isLogin)
                {
                    _headerView.ShowLogo(_fitAndShapeModelApp.FitAndShapeServiceType);
                }
                else
                {
                    _headerView.HideLogo();
                }

                _optionView.Show(IsDemo());

                _loadingView.Visible = true;

                await _webGroupView.HideAsync(_cancellationToken);

                _loadingView.Visible = false;
            }).AddTo(_fitAndShapeView);
        }

        void RegisterOptionEvent()
        {
            _optionView.OnLoginClick.Subscribe(_ => { ShowLogin(); }).AddTo(_fitAndShapeView);

            _optionView.OnInfoClick.Subscribe(async _ =>
            {
                _loadingView.Visible = true;

                await _webGroupView.HideAsync(_cancellationToken);

                SiteType siteType = SiteType.None;

                switch (_fitAndShapeModelApp.FitAndShapeServiceType)
                {
                    case FitAndShapeServiceType.Distortion:
                        siteType = SiteType.PrivacyPolicyDistortion;
                        break;
                    case FitAndShapeServiceType.Measuremenet:
                        siteType = SiteType.PrivacyPolicyMeasurement;
                        break;
                }

                await _webGroupView.ShowAsync(ScreenType.PrivacyPolicy, siteType, _cancellationToken);

                _currentState = DisplayState.PrivacyPolicy;

                _headerView.ShowPreve("戻る");
                _headerView.ShowLogo(FitAndShapeServiceType.None);

                _loadingView.Visible = false;
            }).AddTo(_fitAndShapeView);

            _optionView.OnLogoutClick.Subscribe(_ => { ShowLogin(); }).AddTo(_fitAndShapeView);
        }

        async UniTask ShowScan()
        {
            _loadingView.Visible = true;

            _headerView.HidePreve();
            _headerView.HideExchangeMode();

            _optionView.Hide();

            await _webGroupView.HideAsync(_cancellationToken);

            _currentState = DisplayState.Scan;

            SiteType siteType = SiteType.None;

            switch (_fitAndShapeModelApp.FitAndShapeServiceType)
            {
                case FitAndShapeServiceType.Distortion:
                    siteType = SiteType.ScanDistortion;
                    break;
                case FitAndShapeServiceType.Measuremenet:
                    siteType = SiteType.ScanMeasurement;
                    break;
            }

            await _webGroupView.ShowAsync(ScreenType.Scan, siteType, _cancellationToken);

            _tabBarView.ShowButton(SelectButtonType.Scan);

            _loadingView.Visible = false;
        }

        void Clear()
        {
            LoginInfo.Clear();
            LoginData.Clear();
        }

        void ShowLogin()
        {
            Clear();

            SceneManager.LoadScene(LoginPresenter.SceneName);
        }

        void ShowMyPage()
        {
            HideSelectColorView();
            HideSelectAngleView();
            HideMeasurement();
            HidePostureSummary();

            _headerView.HidePreve();

            _modelView.OnReset();

            switch (_fitAndShapeModelApp.FitAndShapeServiceType)
            {
                case FitAndShapeServiceType.Distortion:
                    ShowPostureSummary();
                    ShowSelectColorView();
                    ShowSelectAngleView();
                    break;
                case FitAndShapeServiceType.Measuremenet:
                    _arrowView.Clear();
                    _arrowView.ArrowPartVisible = true;
                    _measurementView.Clear();
                    ShowMeasurement();
                    ShowSelectColorView();
                    break;
            }
        }

        void HideAll()
        {
            HideSelectColorView();
            HideSelectAngleView();
            HideMeasurement();
            HidePostureSummary();

            _headerView.HidePreve();
            _headerView.HideExchangeMode();

            _optionView.Hide();

            _tabBarView.Clear();

            _selectItemGroupView.Hide();

            _headerView.ShowLogo(FitAndShapeServiceType.None);
        }

        void SetLoginData()
        {
            LoginData loginData = PlayerPrefsUtils.GetObject<LoginData>(LoginData.Key);

            _fitAndShapeInfoView.Show();
            _fitAndShapeInfoView.DateText = loginData.CreatedAt;
            _fitAndShapeInfoView.PlaceText = HttpUtility.UrlDecode(loginData.Place);

            _webGroupView.SetLoginCookies(loginData);

            _waistHistoryView.SetCustomerId(loginData.CustomerID);
        }

        async UniTask<bool> LoadSystemAsync(FitAndShapeServiceType serviceType, string measurementNumber = null)
        {
            //TODO: リフレッシュトークンは存在する？
            LoginData loginData = await AuthManager.Instance.GetLoginData();
            if (measurementNumber != null)
            {
                loginData.SetMeasurementNumber(measurementNumber);
            }

            try
            {
                _loadingView.Visible = true;
                _fitAndShapeModelApp = new FitAndShapeModelApp(serviceType);

                _client = new ApiClient(_fitAndShapeParameter.Host, loginData.MeasurementNumber, string.Empty,
                    loginData.Token);


                ILoadCsvModel loadCsvModel = new LoadCsvModel();

                string measurementUrlString =
                    $"{_fitAndShapeParameter.CsvUrl}/measurements/{loginData.MeasurementNumber}?type=csv&key={_fitAndShapeParameter.ApiKey}";
                var result = await loadCsvModel.GetResult(measurementUrlString, _cancellationToken);

                if (!result.Result)
                {
                    _tabBarView.ShowButton(SelectButtonType.MyPage);
                    return false;
                }

                Csv csv = new Csv();
                csv.SetSourceString(result.CsvData, true);

                IMeasurementCsvLoader measurementCsvLoader = new MeasurementCsvLoader(csv.GetRowValues(0));

                string fileName = "scan_data_hires.obj";
                MemoryStream memoryStream = await _client.Download(fileName);
                Debug.Log($@"[LoadSystemAsync]: {fileName} downloaded");

                // DEBUG: wolf <- what is this?
                // if(loginData.MeasurementNumber == "FS2308086671")
                // {
                //     Debug.Log("[FS2308086671]: download scan_data.fbx start");
                //     _client = new ApiClient(_fitAndShapeParameter.Host, loginData.MeasurementNumber, string.Empty, 
                //         PlayerPrefsUtils.GetObject<LoginData>(LoginData.Key).Token);
                //     memoryStream = await _client.Download("scan_data.fbx");
                //     Debug.Log("[FS2308086671]: download scan_data.fbx end");
                // }             

                _objLines = ObjLoader.LoadAsStream(memoryStream);

                _avatarModel = new AvatarModel(_objLines, _modelView.GetSkeletonTransform(), _modelView.GetBones(),
                    Vector3.zero, true, AppConst.ObjLoadScale);

                _postureVerifyerModel = new PostureVerifyerModel(_avatarModel, measurementCsvLoader);

                _measurementView.CreateMeasurementValueUI(measurementCsvLoader, loginData.Height);

                //switch (_fitAndShapeModelApp.FitAndShapeServiceType)
                //{
                //    case FitAndShapeServiceType.Measuremenet:
                //        _measurementView.Show();
                //        await LoadModelAsync(ColorType.Color, false);
                //        break;
                //    case FitAndShapeServiceType.Distortion:
                //        LoadComment(loadCsvModel, loginData.MeasurementNumber).Forget();
                //        await LoadModelAsync(ColorType.Monochrome, false);
                //        break;
                //}

                switch (currentColor)
                {
                    case 1:
                        //_measurementView.Show();
                        await LoadModelAsync(ColorType.Color, false);
                        break;
                    case 0:
                        LoadComment(loadCsvModel, loginData.MeasurementNumber).Forget();
                        await LoadModelAsync(ColorType.Monochrome, false);
                        break;
                }

                _headerView.ShowLogo(_fitAndShapeModelApp.FitAndShapeServiceType);

                _headerSelectGroupView.SetColorType(
                    _fitAndShapeModelApp.FitAndShapeServiceType == FitAndShapeServiceType.Measuremenet
                        ? SelectItemType.Color
                        : SelectItemType.Monochrome);

                switch (_fitAndShapeModelApp.FitAndShapeServiceType)
                {
                    case FitAndShapeServiceType.Distortion:
                        _currentState = DisplayState.PostureSummary;
                        ShowPostureSummary();
                        ShowSelectColorView();
                        ShowSelectAngleView();
                        SendResult(loginData.MeasurementNumber).Forget();
                        break;

                    case FitAndShapeServiceType.Measuremenet:
                        _currentState = DisplayState.Measurement;
                        ShowMeasurement();
                        ShowSelectColorView();
                        break;
                }

                ShowCommon();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Clear();
                SceneManager.LoadScene("LoadingError");
            }
            finally
            {
                _loadingView.Visible = false;
            }

            return true;
        }

        void ShowCommon()
        {
            _headerView.ShowExchangeMode();

            _tabBarView.ShowButton(SelectButtonType.MyPage);
            _tabBarView.EnabledButton(SelectButtonType.Profile, !IsDemo());
        }

        void ShowSelectColorView()
        {
            _headerSelectGroupView.ColorVisible = true;
        }

        void HideSelectColorView()
        {
            _headerSelectGroupView.ColorVisible = false;
        }

        void ShowSelectAngleView()
        {
            _headerSelectGroupView.AngleVisible = true;
        }

        void HideSelectAngleView()
        {
            _headerSelectGroupView.AngleVisible = false;
        }

        void ShowMeasurement()
        {
            _measurementView.Show();

            _arrowView.Position = Vector3.zero;

            _arrowView.Show();
            _arrowView.SetCameraViewPortRect(_fitAndShapeParameter.MeasurementViewportRect);
            _arrowView.DrawArrow(_avatarModel, AppConst.ObjLoadScale);

            _renderTextureUpdater.SetDisplayCamera(_fitAndShapeView.DefaultAngle);

            Vector3 position = _arrowView.MiddlePosition;

            _modelView.SetPosition(position);

            _arrowView.Position = position;

            var hieghtPosition = _arrowView.GetHieghtPosition(_avatarModel, AppConst.ObjLoadScale);

            float distance = Vector3.Distance(hieghtPosition.StartPosition, hieghtPosition.EndPosition);

            _arrowView.SetFieldOfView(_appParameter.GetFieldOfView(distance));

            Vector3 cameraPosition = _fitAndShapeParameter.CameraPosition;
            cameraPosition.z =
                _fitAndShapeParameter.GetCameraPositionZ(Vector3.Distance(hieghtPosition.StartPosition,
                    hieghtPosition.EndPosition));
            cameraPosition.z = 7.5f;

            _arrowView.SetCameraPosition(cameraPosition);

            _fitAndShapeView.SetPlayerInput();
        }

        void HideMeasurement()
        {
            _measurementView.Hide();
            _arrowView.Hide();
            _waistHistoryView.Hide();

            _fitAndShapeView.DisabledPlayerInput();
        }

        void ShowPostureSummary()
        {
            _arrowView.SetCameraViewPortRect(_fitAndShapeParameter.ViewportRect);

            Result[] results =
                GetResult(_postureVerifyerModel.GetAbnormalResultsByAngle(_fitAndShapeView.DefaultAngle));

            _renderTextureUpdater.ResetCamera();
            _renderTextureUpdater.Show();
            _renderTextureUpdater.SetDisplayCamera(_fitAndShapeView.DefaultAngle);

            RenderTextureController renderTextureController = _renderTextureUpdater.TargetList
                .Where(n => n.Angle == _fitAndShapeView.DefaultAngle).First();

            _posturePageFrameView.Show();
            _posturePageFrameView.CreateWarning(_fitAndShapeView.DefaultAngle, results, renderTextureController,
                _fitAndShapeParameter.PostureWarningOffset);

            _postureSummaryView.Show();
            _postureSummaryView.SetResult(results);

            _postureDetailPageFrame.Show();
            _postureDetailPageFrame.ClearLine();

            foreach (Result result in results)
            {
                _postureDetailPageFrame.DrawMeasurementLine(result);
            }

            _postureArrowView.Hide();
            _postureArrowView.Clear();
            _postureDetailView.Hide();

            _modelView.SetPosition(Vector3.zero);
        }

        void ShowPostureDetail(int number, Result result)
        {
            _posturePageFrameView.Hide();

            _renderTextureUpdater.SetCameraPosition(result, _fitAndShapeView.DefaultAngle, _fitAndShapeParameter);

            _postureSummaryView.Hide();

            _postureDetailPageFrame.Show();
            _postureDetailPageFrame.ClearLine();
            _postureDetailPageFrame.DrawLine(result);

            _postureArrowView.Show();
            _postureArrowView.Clear();
            _postureArrowView.DrawArrow(result, _fitAndShapeView.DefaultAngle,
                _renderTextureUpdater.GetDisplayCamera(_fitAndShapeView.DefaultAngle));

            _postureDetailView.Show();
            _postureDetailView.SetResult(number, result, _commentModel?.GetComment(result.Point),
                _postureAdviceAsset.GetEntity(result.Point));

            _headerView.ShowPreve(result.Summary, number.ToString());
            _headerView.ShowLogo(FitAndShapeServiceType.None);
        }

        async UniTask SendResult(string measurementNumber)
        {
            Result[] results = _postureVerifyerModel.GetAbnormalResults();

            List<BodyDistortionsEntity> bodyDistortionsList = new List<BodyDistortionsEntity>();

            foreach (Result result in results)
            {
                BodyDistortionsEntity bodyDistortionsEntity = new BodyDistortionsEntity(result.Point, result.Condition);

                if (bodyDistortionsList.Contains(bodyDistortionsEntity))
                {
                    continue;
                }

                bodyDistortionsList.Add(bodyDistortionsEntity);
            }

            string json = JsonHelper.ToJson(bodyDistortionsList);

            string urlString =
                $"{_fitAndShapeParameter.CsvUrl}/measurements/{measurementNumber.Substring(2, measurementNumber.Length - 2)}/body-distortions?key={_fitAndShapeParameter.ApiKey}";

            ISendResultModel sendResultModel = new SendResultModel();

            await sendResultModel.Put(urlString, json, _cancellationToken);
        }

        Result[] GetResult(Result[] results)
        {
            List<Result> resultList = new List<Result>();

            foreach (Result result in results)
            {
                PostureAdviceEntity postureAdviceEntity = _postureAdviceAsset.GetEntity(result.Point);

                if (postureAdviceEntity == null) continue;

                if (postureAdviceEntity.PostureAdvicePoint == PostureAdvicePoint.None) continue;

                resultList.Add(result);
            }

            return resultList.ToArray();
        }

        void HidePostureSummary()
        {
            _postureSummaryView.Hide();
            _postureDetailView.Hide();
            _posturePageFrameView.Hide();
            _postureDetailPageFrame.Hide();
            _postureArrowView.Hide();
            _renderTextureUpdater.Hide();
        }

        public void _Color()
        {
            _selectItemGroupView.OnButtonClick.Subscribe(async n => //Here!
            {
                _headerSelectGroupView.SetSelectItemType(n);

                switch (n.ToSelectType())
                {
                    case SelectType.Color:
                        ColorType colorType = n.ToColorType();
                        await LoadModelAsync(colorType, true);
                        break;
                    default:
                        _fitAndShapeView.DefaultAngle = n.ToAngle();
                        ShowPostureSummary();
                        break;
                }
            }).AddTo(_fitAndShapeView);
        }
    }
}