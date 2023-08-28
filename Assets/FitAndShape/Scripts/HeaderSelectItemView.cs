using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FitAndShape
{
    public sealed class HeaderSelectItemView : MonoBehaviour
    {
        [SerializeField] Button _button;
        [SerializeField] TextMeshProUGUI _text;

        public IObservable<SelectItemType> OnButtonClick => _onButtonClick;
        Subject<SelectItemType> _onButtonClick = new Subject<SelectItemType>();

        public SelectItemType SelectItemType { get; private set; }

        public void Initialize(SelectItemType selectItemType)
        {
            SetSelectItemType(selectItemType);

            _button.OnClickAsObservable().Subscribe(_ => _onButtonClick.OnNext(SelectItemType)).AddTo(this);
        }

        public void SetSelectItemType(SelectItemType selectItemType)
        {
            SelectItemType = selectItemType;

            _text.text = selectItemType.ToName();
        }
    }
}