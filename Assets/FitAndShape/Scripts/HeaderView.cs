using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Amatib.ObjViewer.Domain;

namespace FitAndShape
{
    public sealed class HeaderView : MonoBehaviour
    {
        [SerializeField] Button _exchangeModeButton;
        [SerializeField] Button _preveButton;
        [SerializeField] TextMeshProUGUI _pageTitleText;
        [SerializeField] Sprite _logoMeasurementSprite;
        [SerializeField] Sprite _logoPostureSprite;
        [SerializeField] Image _logImage;
        [SerializeField] Image _iconImage;
        [SerializeField] NumberView _numberView;

        public IObservable<Unit> OnExchangeModeClick => _onExchangeModeClick;
        Subject<Unit> _onExchangeModeClick = new Subject<Unit>();

        public IObservable<Unit> OnPreveClick => _onPreveClick;
        Subject<Unit> _onPreveClick = new Subject<Unit>();

        public void Initialize()
        {
            if (_exchangeModeButton != null)
            {
                _exchangeModeButton.OnClickAsObservable().Subscribe(_ => _onExchangeModeClick.OnNext(Unit.Default)).AddTo(this);
            }

            if (_preveButton != null)
            {
                _preveButton.OnClickAsObservable().Subscribe(_ => _onPreveClick.OnNext(Unit.Default)).AddTo(this);
            }

            if (_logImage != null)
            {
                _logImage.SetNativeSize();
            }

            HidePreve();
        }

        public void ShowPreve(string title, string numberText = "")
        {
            if (_numberView != null)
            {
                _numberView.Text = numberText;
            }
            
            _pageTitleText.text = title;
            _preveButton.gameObject.SetActive(true);
            _pageTitleText.gameObject.SetActive(true);
        }

        public void HidePreve()
        {
            if (_numberView != null)
            {
                _numberView.Text = string.Empty;
            }

            if (_preveButton != null)
            {
                _preveButton.gameObject.SetActive(false);
            }

            if (_pageTitleText != null)
            {
                _pageTitleText.gameObject.SetActive(false);
            }
        }

        public void ShowExchangeMode()
        {
            _exchangeModeButton.gameObject.SetActive(true);
        }

        public void HideExchangeMode()
        {
            _exchangeModeButton.gameObject.SetActive(false);
        }

        public void ShowLogo(FitAndShapeServiceType fitAndShapeServiceType)
        {
            switch (fitAndShapeServiceType)
            {
                case FitAndShapeServiceType.Distortion:
                    _logImage.sprite = _logoPostureSprite;
                    break;
                case FitAndShapeServiceType.Measuremenet:
                    _logImage.sprite = _logoMeasurementSprite;
                    break;
            }

            _logImage.SetNativeSize();

            switch (fitAndShapeServiceType)
            {
                case FitAndShapeServiceType.Distortion:
                case FitAndShapeServiceType.Measuremenet:
                    _logImage.gameObject.SetActive(true);
                    _iconImage.gameObject.SetActive(true);
                    break;
                default:
                    _logImage.gameObject.SetActive(false);
                    _iconImage.gameObject.SetActive(false);
                    break;
            }
        }

        public void HideLogo()
        {
            _logImage.gameObject.SetActive(false);
            _iconImage.gameObject.SetActive(false);
        }
    }
}