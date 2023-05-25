using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FitAndShape
{
    public sealed class WaistHistoryView : MonoBehaviour
    {
        [SerializeField] Button _prevButton;
        [SerializeField] Button _pagePrevButton;
        [SerializeField] Button _pageNextButton;
        [SerializeField] RectTransform _rectTransform;
        [SerializeField] WaistValueUI _waistValueUIPrefab;
        [SerializeField] PaginationView _paginationView;
        [SerializeField] TextMeshProUGUI _titleText;
        [SerializeField] Color _enabledColor;
        [SerializeField] Color _disabledColor;
        [SerializeField] Image _pagePrevImage;
        [SerializeField] Image _pageNextImage;

        public IObservable<Unit> OnPrevClick => _onPrevClick;
        Subject<Unit> _onPrevClick = new Subject<Unit>();

        MeasurementPart _measurementPart;

        string _baseUrl;
        string _customerId;
        string _apiKey;
        ILoadCsvModel _loadCsvModel;

        List<WaistValueUI> _waistValueUIList = new List<WaistValueUI>();

        int _currentPage;
        int _maxPage;
        bool _isLoad = false;

        public void Initialize(string baseUrl, string apiKey)
        {
            Initialize(baseUrl, string.Empty, apiKey);
        }

        public void SetCustomerId(string customerId)
        {
            _customerId = customerId;
        }

        public void Initialize(string baseUrl, string customerId, string apiKey)
        {
            _baseUrl = baseUrl;
            _customerId = customerId;
            _apiKey = apiKey;

            gameObject.SetActive(false);

            _loadCsvModel = new LoadCsvModel();

            _prevButton.OnClickAsObservable().Subscribe(_ =>
            {
                _onPrevClick.OnNext(Unit.Default);

                Hide();

            }).AddTo(this);

            _pagePrevButton.OnClickAsObservable().Where(_ => !_isLoad).Subscribe(_ =>
            {
                PrevPage().Forget();

            }).AddTo(this);

            _pageNextButton.OnClickAsObservable().Where(_ => !_isLoad).Subscribe(_ =>
            {
                NextPage().Forget();

            }).AddTo(this);

            _paginationView.OnClick.Where(_ => !_isLoad).Subscribe(n =>
            {
                _currentPage = n;

                SetButtonColor();

                LoadPage().Forget();

            }).AddTo(this);
        }

        public string GetTitle(MeasurementPart measurementPart)
        {
            switch (measurementPart)
            {
                case MeasurementPart.WaistMaxCircumference:
                    return "ウエスト（最大）の測定履歴";
                case MeasurementPart.WaistMinCircumference:
                    return "ウエスト（最小）の測定履歴";
                default:
                    return string.Empty;
            }
        }


        public async void Show(MeasurementPart measurementPart)
        {
            _titleText.text = GetTitle(measurementPart);

            _isLoad = true;

            Clear();

            _currentPage = 1;

            _measurementPart = measurementPart;

            gameObject.SetActive(true);

            string urlString = GetUrl(_measurementPart, _currentPage);

            WaistHistoryEntity waistHistoryEntity = await Load(urlString);

            _maxPage = waistHistoryEntity.LastPage;

            Create(waistHistoryEntity);

            _isLoad = false;

            _paginationView.Create(_maxPage);

            SetButtonColor();
        }

        void Clear()
        {
            foreach (WaistValueUI waistValueUI in _waistValueUIList)
            {
                Destroy(waistValueUI.gameObject);
            }

            _waistValueUIList.Clear();
        }

        void Create(WaistHistoryEntity waistHistoryEntity)
        {
            foreach (WasitHistoryValue wasitHistoryValue in waistHistoryEntity.Data)
            {
                WaistValueUI waistValueUI = Instantiate(_waistValueUIPrefab, _rectTransform);
                waistValueUI.DateText = wasitHistoryValue.CreatedAt;
                waistValueUI.ValueText = $"{(float)wasitHistoryValue.Waist / 10f}";

                _waistValueUIList.Add(waistValueUI);
            }
        }

        string GetUrl(MeasurementPart measurementPart, int page)
        {
            string apiName = string.Empty;

            switch (measurementPart)
            {
                case MeasurementPart.WaistMaxCircumference:
                    apiName = "maximum-waist-history";
                    break;
                case MeasurementPart.WaistMinCircumference:
                    apiName = "minimum-waist-history";
                    break;
            }

            return $"{_baseUrl}/customers/{_customerId}/{apiName}?key={_apiKey}&page={page}";
        }

        async UniTask<WaistHistoryEntity> Load(string urlString)
        {
            string waistHistory = await _loadCsvModel.GetCsvData(urlString, this.GetCancellationTokenOnDestroy());

            WaistHistoryEntity waistHistoryEntity = JsonUtility.FromJson<WaistHistoryEntity>(waistHistory);

            return waistHistoryEntity;
        }


        public async UniTask PrevPage()
        {
            if (_currentPage - 1 < 1)
            {
                return;
            }

            _currentPage--;

            SetButtonColor();

            await LoadPage();
        }

        public async UniTask NextPage()
        {
            if (_currentPage + 1 > _maxPage)
            {
                return;
            }

            _currentPage++;

            SetButtonColor();

            await LoadPage();
        }

        void SetButtonColor()
        {
            if (_currentPage == 1)
            {
                _pagePrevImage.color = _disabledColor;
            }
            else
            {
                _pagePrevImage.color = _enabledColor;
            }

            if (_currentPage == _maxPage)
            {
                _pageNextImage.color = _disabledColor;
            }
            else
            {
                _pageNextImage.color = _enabledColor;
            }
        }

        async UniTask LoadPage()
        {
            _isLoad = true;

            Clear();

            string urlString = GetUrl(_measurementPart, _currentPage);

            WaistHistoryEntity waistHistoryEntity = await Load(urlString);

            Create(waistHistoryEntity);

            _paginationView.UpdatePagination(_currentPage);

            _isLoad = false;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}