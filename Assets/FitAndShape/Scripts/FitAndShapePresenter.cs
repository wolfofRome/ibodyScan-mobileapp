using Amatib.ObjViewer.Domain;
using Amatib.ObjViewer.Infrastructure;
using Amatib.ObjViewer.Presentation.Loaders;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;


namespace FitAndShape
{
    public sealed class FitAndShapePresenter : IInitializable
    {
        [Inject] readonly FitAndShapeView _fitAndShapeView;
        [Inject] readonly Parameter _parameter;
        [Inject] readonly FitAndShapeParameter _fitAndShapeParameter;
        [Inject] readonly ArrowView _arrowView;
        [Inject] readonly ModelView _modelView;
        [Inject] readonly AvatarView _avatarView;
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
        [Inject] readonly SelectGroupView _selectGroupView;

        IAvatarModel _avatarModel;
        IPostureVerifyerModel _postureVerifyerModel;
        ICommentModel _commentModel;
        CancellationToken _token;
        ApiClient _client;
        Vector3 _scale;
        string[] _objLines;

        async void IInitializable.Initialize()
        {
            try
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                _token = cancellationTokenSource.Token;

                _fitAndShapeView.CameraVisible = true;
                _loadingView.Visible = true;

                Initialize();

                HideSelectColorView();
                HideSelectAngleView();
                HideMeasurement();
                HidePostureSummary();

                _client = new ApiClient(_parameter.ApiHost, _parameter.MeasurementNumber, _parameter.Key, _parameter.Token);

                _scale = _fitAndShapeParameter.Scale;

                ILoadCsvModel loadCsvModel = new LoadCsvModel();

                IMeasurementCsvLoader measurementCsvLoader = await GetMeasurementCsvLoader(loadCsvModel);

                MemoryStream memoryStream = await _client.Download("scan_data_hires.obj");

                _objLines = ObjLoader.LoadAsStream(memoryStream);

                SetObjLines(_objLines, measurementCsvLoader);

                switch (_parameter.FitAndShapeServiceType)
                {
                    case FitAndShapeServiceType.Measuremenet:

                        _measurementView.gameObject.SetActive(true);

                        _selectGroupView.SetMeasurementPosition();

                        await LoadModel(ColorType.Color, false);

                        break;

                    case FitAndShapeServiceType.Distortion:

                        LoadComment(loadCsvModel).Forget();

                        await LoadModel(ColorType.Monochrome, false);

                        break;
                }

                _selectGroupView.SelectViewColor.SelectToggleUI(_parameter.FitAndShapeServiceType == FitAndShapeServiceType.Measuremenet ? 0 : 1);

                RegisterEvent();

                _loadingView.Visible = false;

                _fitAndShapeView.CameraVisible = false;

                switch (_parameter.FitAndShapeServiceType)
                {
                    case FitAndShapeServiceType.Distortion:
                        ShowPostureSummary();
                        ShowSelectColorView();
                        ShowSelectAngleView();
                        SendResult().Forget();
                        break;

                    case FitAndShapeServiceType.Measuremenet:
                        ShowMeasurement();
                        ShowSelectColorView();
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                SceneManager.LoadScene("LoadingError");
            }
            finally
            {
                _loadingView.Visible = false;
            }
        }

        async UniTask LoadModel(ColorType colorType, bool showLoading)
        {
            try
            {
                if (showLoading)
                {
                    _loadingView.Visible = true;
                }

                switch (colorType)
                {
                    case ColorType.Color:

                        if (!_modelView.IsLoadPointCloud)
                        {
                            _modelView.SetPointCloudMesh(PlyLoader.LoadAsMesh(await _client.Download("scan_data.ply"), _scale));
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

        async UniTask<IMeasurementCsvLoader> GetMeasurementCsvLoader(ILoadCsvModel loadCsvModel)
        {
            string measurementUrlString = $"{_fitAndShapeParameter.CsvUrl}/measurements/{_parameter.MeasurementNumber}?type=csv&key={_fitAndShapeParameter.ApiKey}";
            string csvData = await loadCsvModel.GetCsvData(measurementUrlString, _token);

            Csv csv = new Csv();
            csv.SetSourceString(csvData, true);

            return new MeasurementCsvLoader(csv.GetRowValues(0));
        }

        async UniTask LoadComment(ILoadCsvModel loadCsvModel)
        {
            string urlString = $"{_fitAndShapeParameter.CsvUrl}/measurements/{_parameter.MeasurementNumber.Substring(2, _parameter.MeasurementNumber.Length - 2)}/body-distortion-comments?key={_fitAndShapeParameter.ApiKey}";
            string commentData = await loadCsvModel.GetCsvData(urlString, _token);
            _commentModel = new CommentModel(commentData);
        }

        void SetObjLines(string[] objLines, IMeasurementCsvLoader measurementCsvLoader)
        {
            Transform[] bones = _avatarView.GetBones();

            _avatarModel = new AvatarModel(objLines, bones, Vector3.zero, true, AppConst.ObjLoadScale);

            _postureVerifyerModel = new PostureVerifyerModel(_avatarModel, measurementCsvLoader);

            _measurementView.CreateMeasurementValueUI(measurementCsvLoader, _parameter.Height);
        }

        void Initialize()
        {
            _renderTextureUpdater.Initialize(_fitAndShapeParameter.FieldOfView);
            _renderTextureUpdater.SetDisplayCamera(_fitAndShapeView.DefaultAngle);
            _renderTextureUpdater.SetViewPortRect(_fitAndShapeParameter.ViewportRect);

            _arrowView.SetCameraViewPortRect(_fitAndShapeParameter.MeasurementViewportRect);

            _postureDetailView.Initialize();

            _selectGroupView.Initialize();

            _waistHistoryView.Initialize(_fitAndShapeParameter.CsvUrl, _parameter.CustomerId, _fitAndShapeParameter.ApiKey);

            _postureSummaryView.SetParameter(_parameter);
            _postureDetailView.SetParameter(_parameter);
            _measurementView.SetParameter(_parameter);
        }

        void RegisterEvent()
        {
            _posturePageFrameView.OnClickAsWarningButton.Subscribe(value =>
            {
                ShowPostureDetail(value.Number, value.Result);

            }).AddTo(_fitAndShapeView);

            _postureSummaryView.OnClick.Subscribe(value =>
            {
                ShowPostureDetail(value.Number, value.Result);

            }).AddTo(_fitAndShapeView);

            _postureDetailView.OnPreveButtonClick.Subscribe(_ =>
            {
                ShowPostureSummary();

            }).AddTo(_fitAndShapeView);

            _selectGroupView.SelectViewAngle.OnValueChanged.Subscribe(value =>
            {
                _fitAndShapeView.DefaultAngle = (Angle)value;

                ShowPostureSummary();

            }).AddTo(_fitAndShapeView);

            _selectGroupView.SelectViewColor.OnValueChanged.Subscribe(async value =>
            {
                ColorType colorType = (ColorType)value;

                await LoadModel(colorType, true);

            }).AddTo(_fitAndShapeView);

            _measurementView.OnClick.Subscribe(x =>
            {
                _arrowView.Select(x);

                _arrowView.OnReset();
                _arrowView.ArrowPartVisible = true;

                _modelView.OnReset();

            }).AddTo(_fitAndShapeView);

            _measurementView.OnHistoryClick.Subscribe(x =>
            {
                _arrowView.Select(x);

                _arrowView.OnReset();
                _arrowView.ArrowPartVisible = true;

                _modelView.OnReset();

                switch (x)
                {
                    case MeasurementPart.WaistMaxCircumference:
                    case MeasurementPart.WaistMinCircumference:
                        _waistHistoryView.Show(x);
                        break;
                }

            }).AddTo(_fitAndShapeView);

            _fitAndShapeView.OnPlayerInput.Subscribe(_ =>
            {
                _arrowView.ArrowPartVisible = false;

            }).AddTo(_fitAndShapeView);

            _waistHistoryView.OnPrevClick.Subscribe(_ =>
            {
                _arrowView.OnReset();
                _arrowView.ArrowPartVisible = true;

                _modelView.OnReset();

            }).AddTo(_fitAndShapeView);
        }

        void ShowSelectColorView()
        {
            _selectGroupView.SelectViewColor.gameObject.SetActive(true);
        }

        void HideSelectColorView()
        {
            _selectGroupView.SelectViewColor.gameObject.SetActive(false);
        }

        void ShowSelectAngleView()
        {
            _selectGroupView.SelectViewAngle.gameObject.SetActive(true);
        }

        void HideSelectAngleView()
        {
            _selectGroupView.SelectViewAngle.gameObject.SetActive(false);
        }

        void ShowMeasurement()
        {
            _arrowView.Show();

            _fitAndShapeView.DefaultAngle = Angle.Front;
            _renderTextureUpdater.SetDisplayCamera(_fitAndShapeView.DefaultAngle);

            _arrowView.DrawArrow(_avatarModel, AppConst.ObjLoadScale);

            Vector3 position = _arrowView.MiddlePosition;

            _modelView.SetPosition(position);

            var hieghtPosition = _arrowView.GetHieghtPosition(_avatarModel, AppConst.ObjLoadScale);

            Vector3 cameraPosition = _fitAndShapeParameter.CameraPosition;
            cameraPosition.z = _fitAndShapeParameter.GetCameraPositionZ(Vector3.Distance(hieghtPosition.StartPosition, hieghtPosition.EndPosition));

            _arrowView.SetZoomValue();
            _arrowView.SetCameraPosition(cameraPosition);
            _arrowView.SetArrowPartPosition(position);

            _fitAndShapeView.SetPlayerInput();
        }

        void HideMeasurement()
        {
            _measurementView.gameObject.SetActive(false);
        }

        void ShowPostureDetail(int number, Result result)
        {
            _posturePageFrameView.ShowWarning(false);

            _renderTextureUpdater.SetCameraPosition(result, _fitAndShapeView.DefaultAngle, _fitAndShapeParameter);

            _postureSummaryView.gameObject.SetActive(false);

            _postureDetailPageFrame.ClearLine();
            _postureDetailPageFrame.DrawLine(result);

            _postureArrowView.gameObject.SetActive(true);
            _postureArrowView.Clear();
            _postureArrowView.DrawArrow(result, _fitAndShapeView.DefaultAngle, _renderTextureUpdater.GetDisplayCamera(_fitAndShapeView.DefaultAngle));

            _postureDetailView.gameObject.SetActive(true);
            _postureDetailView.SetResult(number, result, _commentModel?.GetComment(result.Point), _postureAdviceAsset.GetEntity(result.Point));
        }

        void ShowPostureSummary()
        {
            _renderTextureUpdater.ResetCamera();

            Result[] results = GetResult(_postureVerifyerModel.GetAbnormalResultsByAngle(_fitAndShapeView.DefaultAngle));

            _renderTextureUpdater.gameObject.SetActive(true);
            _renderTextureUpdater.SetDisplayCamera(_fitAndShapeView.DefaultAngle);

            RenderTextureController renderTextureController = _renderTextureUpdater.TargetList.Where(n => n.Angle == _fitAndShapeView.DefaultAngle).First();

            _posturePageFrameView.CreateWarning(_fitAndShapeView.DefaultAngle, results, renderTextureController, _fitAndShapeParameter.PostureWarningOffset);
            _posturePageFrameView.ShowWarning(true);

            _postureSummaryView.gameObject.SetActive(true);
            _postureSummaryView.SetResult(results);

            _postureDetailPageFrame.ClearLine();

            foreach (Result result in results)
            {
                _postureDetailPageFrame.DrawMeasurementLine(result);
            }

            _postureArrowView.gameObject.SetActive(false);
            _postureArrowView.Clear();

            _postureDetailView.gameObject.SetActive(false);
        }

        async UniTask SendResult()
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

            string urlString = $"{_fitAndShapeParameter.CsvUrl}/measurements/{_parameter.MeasurementNumber.Substring(2, _parameter.MeasurementNumber.Length - 2)}/body-distortions?key={_fitAndShapeParameter.ApiKey}";

            ISendResultModel sendResultModel = new SendResultModel();

            await sendResultModel.Put(urlString, json, _token);
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
            _postureSummaryView.gameObject.SetActive(false);

            _postureDetailView.gameObject.SetActive(false);
        }
    }
}