using System;
using Amatib.ObjViewer.Domain;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FitAndShape
{
    public sealed class ServiceTypeView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] Image _image;
        [SerializeField] TextMeshProUGUI _text;
        [SerializeField] Color _selectFontColor;
        [SerializeField] Color _selectImageColor;
        [SerializeField] Color _defaultFontColor;
        [SerializeField] Color _defaultImageColor;
        [SerializeField] FitAndShapeServiceType _fitAndShapeServiceType;
        [SerializeField] Image _logoImage;
        [SerializeField] Sprite _selectLogoSprite;
        [SerializeField] Sprite _defaultLogoSprite;

        public FitAndShapeServiceType FitAndShapeServiceType => _fitAndShapeServiceType;

        public IObservable<ServiceTypeView> OnClick => _onClick;
        Subject<ServiceTypeView> _onClick = new Subject<ServiceTypeView>();

        public bool IsSelect { get; private set; }

        public void Select(bool value)
        {
            if (value)
            {
                _text.color = _selectFontColor;
                _image.color = _selectImageColor;
                _logoImage.sprite = _selectLogoSprite;
            }
            else
            {
                _text.color = _defaultFontColor;
                _image.color = _defaultImageColor;
                _logoImage.sprite = _defaultLogoSprite;
            }

            IsSelect = value;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClick.OnNext(this);
        }
    }
}