using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FitAndShape
{
    public sealed class ToggleUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] Image _image;

        public IObservable<int> OnClick => _onClick;
        Subject<int> _onClick = new Subject<int>();

        bool _select = false;
        int _index = -1;

        public int Index
        {
            get
            {
                if (_index < 0)
                {
                    int.TryParse(gameObject.name.Substring(gameObject.name.Length - 1, 1), out _index);
                }

                return _index;
            }
        }

        public bool Select
        {
            get { return _select; }
            set
            {
                _select = value;
                _image.enabled = _select;
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            _onClick.OnNext(Index);
        }
    }
}