using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FitAndShape
{
    public enum SelectButtonType
    {
        None,
        MyPage,
        Scan,
        Profile,
        Option,
    }

    public sealed class SelectButtonView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _text;
        [SerializeField] Sprite _defaultSprite;
        [SerializeField] Sprite _selectSprite;
        [SerializeField] string _title;
        [SerializeField] Button _button;
        [SerializeField] Image _image;
        [SerializeField] SelectButtonType _selectButtonType;
        [SerializeField] Color _defaultColor;
        [SerializeField] Color _selectColor;

        public IObservable<Unit> OnClick => _click;
        Subject<Unit> _click = new Subject<Unit>();

        public SelectButtonType SelectButtonType => _selectButtonType;

        public bool IsActive { get; private set; } = false;

        public bool IsEnabled { get; set; } = true;

        public void Initialize()
        {
            _text.text = _title;
            _button.OnClickAsObservable().Subscribe(_ => _click.OnNext(Unit.Default)).AddTo(this);
            NonActive();
        }

        public void Active()
        {
            IsActive = true;
            _image.sprite = _selectSprite;
            _text.color = _selectColor;
        }

        public void NonActive()
        {
            IsActive = false;
            _image.sprite = _defaultSprite;
            _text.color = _defaultColor;
        }
    }
}