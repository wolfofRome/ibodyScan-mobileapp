using System;
using UniRx;
using UnityEngine;

namespace FitAndShape
{
    public sealed class HeaderSelectGroupView : MonoBehaviour
    {
        [SerializeField] HeaderSelectItemView _colorHeaderSelectItemView;
        [SerializeField] HeaderSelectItemView _angleHeaderSelectItemView;
        [SerializeField] GameObject _colorGameObject;
        [SerializeField] GameObject _angleGameObject;

        public bool ColorVisible { set { _colorGameObject.SetActive(value); } }
        public bool AngleVisible { set { _angleGameObject.SetActive(value); } }

        public IObservable<(SelectType SelectType, SelectItemType SelectItemType)> OnButtonClick => _onButtonClick;
        Subject<(SelectType, SelectItemType)> _onButtonClick = new Subject<(SelectType, SelectItemType)>();

        public void Initialize()
        {
            _colorHeaderSelectItemView.Initialize(SelectItemType.Color);
            _angleHeaderSelectItemView.Initialize(SelectItemType.Front);

            _colorHeaderSelectItemView.OnButtonClick.Subscribe(n => _onButtonClick.OnNext((SelectType.Color, n))).AddTo(this);
            _angleHeaderSelectItemView.OnButtonClick.Subscribe(n => _onButtonClick.OnNext((SelectType.Angle, n))).AddTo(this);
        }

        public void SetColorType(SelectItemType selectItemType)
        {
            _colorHeaderSelectItemView.SetSelectItemType(selectItemType);
        }

        public void SetAngleType(SelectItemType selectItemType)
        {
            _angleHeaderSelectItemView.SetSelectItemType(selectItemType);
        }

        public void SetSelectItemType(SelectItemType selectItemType)
        {
            switch (selectItemType.ToSelectType())
            {
                case SelectType.Color:
                    _colorHeaderSelectItemView.SetSelectItemType(selectItemType);
                    break;
                default:
                    _angleHeaderSelectItemView.SetSelectItemType(selectItemType);
                    break;
            }
        }
    }
}