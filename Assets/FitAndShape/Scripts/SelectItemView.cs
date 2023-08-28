using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FitAndShape
{
    public sealed class SelectItemView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _text;
        [SerializeField] Button _button;
        [SerializeField] Image _image;

        public SelectItemType SelectItemType { get; private set; }

        public bool Selected { get; set; } = false;

        public IObservable<SelectItemView> OnButtonClick => _onButtonClick;
        Subject<SelectItemView> _onButtonClick = new Subject<SelectItemView>();

        public void Initialize(SelectItemType selectItemType)
        {
            SelectItemType = selectItemType;

            _text.text = SelectItemType.ToName();

            _button.OnClickAsObservable().Subscribe(_ => _onButtonClick.OnNext(this)).AddTo(this);
        }

        public void SetColor(Color color)
        {
            _image.color = color;
        }
    }
}