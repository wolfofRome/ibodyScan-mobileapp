using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace FitAndShape
{
    public sealed class SelectItemGroupView : MonoBehaviour
    {
        [SerializeField] Transform _transform;
        [SerializeField] Button _button;
        [SerializeField] Button _closeButton;
        [SerializeField] Button _backgroundCloseButton;
        [SerializeField] SelectItemView _selectItemViewPrefab;
        [SerializeField] Color _defaultColor;
        [SerializeField] Color _color;
        [SerializeField] RectTransform _rectTransform;
        [SerializeField] float _moveDuration;

        public IObservable<SelectItemType> OnButtonClick => _onButtonClick;
        Subject<SelectItemType> _onButtonClick = new Subject<SelectItemType>();

        List<SelectItemView> _selectItemViewList = new List<SelectItemView>();

        bool _isAnimation = false;

        public void Initialize()
        {
            _button.OnClickAsObservable().Where(_ => !_isAnimation).Subscribe(_ =>
            {
                var item = _selectItemViewList.Where(n => n.Selected).FirstOrDefault();

                if (item == null)
                {
                    return;
                }

                _onButtonClick.OnNext(item.SelectItemType);

                HideAnimation();

            }).AddTo(this);

            _closeButton.OnClickAsObservable().Where(_ => !_isAnimation).Subscribe(_ => HideAnimation()).AddTo(this);
            _backgroundCloseButton.OnClickAsObservable().Where(_ => !_isAnimation).Subscribe(_ => HideAnimation()).AddTo(this);
        }

        public void Show(SelectType selectType, SelectItemType selectItemType)
        {
            _isAnimation = true;

            Clear();

            switch (selectType)
            {
                case SelectType.Angle:
                    AddAngle();
                    break;
                case SelectType.Color:
                    AddColor();
                    break;
                default:
                    break;
            }

            var item = _selectItemViewList.Where(n => n.SelectItemType == selectItemType).FirstOrDefault();
            item.Selected = true;
            item.SetColor(_color);

            _rectTransform.DOLocalMoveY(0, _moveDuration).OnComplete(OnComplete);
        }

        void OnComplete()
        {
            _isAnimation = false;
        }

        void Clear()
        {
            foreach (var item in _selectItemViewList)
            {
                Destroy(item.gameObject);
            }

            _selectItemViewList.Clear();
        }

        void AddColor()
        {
            Add(SelectItemType.Color);
            Add(SelectItemType.Monochrome);
        }

        void AddAngle()
        {
            Add(SelectItemType.Front);
            Add(SelectItemType.Back);
            Add(SelectItemType.Top);
            Add(SelectItemType.Under);
            Add(SelectItemType.Left);
            Add(SelectItemType.Right);
        }

        void Add(SelectItemType selectItemType)
        {
            SelectItemView item = Instantiate(_selectItemViewPrefab, _transform);
            item.Initialize(selectItemType);
            item.OnButtonClick.Subscribe(n =>
            {
                foreach (var item in _selectItemViewList)
                {
                    item.SetColor(_defaultColor);
                    item.Selected = false;
                }

                n.SetColor(_color);
                n.Selected = true;

            }).AddTo(this);

            _selectItemViewList.Add(item);

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void HideAnimation()
        {
            _isAnimation = true;

            _rectTransform.DOLocalMoveY(-900, _moveDuration).OnComplete(() => 
            {
                OnComplete();

                Hide();
            });
        }
    }
}