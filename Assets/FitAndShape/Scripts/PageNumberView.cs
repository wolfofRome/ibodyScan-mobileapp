using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FitAndShape
{
    public sealed class PageNumberView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] TextMeshProUGUI _numberText;
        [SerializeField] GameObject _selectedGameObject;

        public IObservable<int> OnClick => _onClick;
        Subject<int> _onClick = new Subject<int>();

        int _page;
        bool _selected;

        public int Page
        {
            get { return _page; }
            set
            {
                _page = value;

                _numberText.text = $"{_page}";
            }
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;

                _selectedGameObject.SetActive(_selected);
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            _onClick.OnNext(_page);
        }
    }
}