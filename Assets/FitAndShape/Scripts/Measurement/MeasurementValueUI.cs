using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FitAndShape
{
    public sealed class MeasurementValueUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] TextMeshProUGUI _nameText;
        [SerializeField] TextMeshProUGUI _valueText;
        [SerializeField] TextMeshProUGUI _unitText;
        [SerializeField] Image _backgroundImage;
        [SerializeField] Image _checkImage;
        [SerializeField] Sprite _checkSprite;
        [SerializeField] Sprite _checkedPprite;
        [SerializeField] Button _historyButton;

        [SerializeField] Color _defaultColor;
        [SerializeField] Color _selectColor;
        [SerializeField] Color _nameColor;
        [SerializeField] Color _valueColor;
        [SerializeField] Color _unitColor;

        public IObservable<MeasurementPart> OnClick => _onClick;
        Subject<MeasurementPart> _onClick = new Subject<MeasurementPart>();

        public IObservable<MeasurementPart> OnHistoryClick => _onHistoryClick;
        Subject<MeasurementPart> _onHistoryClick = new Subject<MeasurementPart>();

        bool _isSelect = false;

        public bool Select
        {
            get { return _isSelect; }
            set
            {
                _isSelect = value;

                _backgroundImage.color = _isSelect ? _selectColor : _defaultColor;
                _checkImage.sprite = _isSelect ? _checkedPprite : _checkSprite;
            }
        }

        public string Name { set { _nameText.text = value; } }
        public float Value { set { _valueText.text = $"{value}"; } }
        public MeasurementPart MeasurementPart { get; private set; }

        public void Initialize(MeasurementPart measurementPart)
        {
            MeasurementPart = measurementPart;

            if (_historyButton != null)
            {
                _historyButton.OnClickAsObservable().Subscribe(_ =>
                {
                    _onHistoryClick.OnNext(MeasurementPart);

                }).AddTo(this);
            }
        }

        public bool HistoryButtonVisible
        {
            get
            {
                if (_historyButton == null)
                {
                    return false;
                }

                return _historyButton.gameObject.activeSelf;
            }
            set
            {
                if (_historyButton == null)
                {
                    return;
                }

                _historyButton.gameObject.SetActive(value);
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            _onClick.OnNext(MeasurementPart);
        }
    }
}