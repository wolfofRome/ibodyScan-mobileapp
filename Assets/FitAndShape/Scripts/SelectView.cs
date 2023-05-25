using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;

namespace FitAndShape
{
    public sealed class SelectView : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown _dropdown;
        [SerializeField] List<ToggleUI> _toggleUIList;
        [SerializeField] int _defaultIndex;
        [SerializeField] DropdownType _dropdownType;

        bool _isInitialize = false;

        public Subject<int> OnValueChanged { get; } = new Subject<int>();
        public bool Visible { get { return gameObject.activeSelf; } set { gameObject.SetActive(value); } }

        public void Initialize()
        {
            if (_isInitialize) return;

            gameObject.SetActive(false);

            _isInitialize = true;

            SelectToggleUI(_defaultIndex);

            foreach (var toggle in _toggleUIList)
            {
                toggle.OnClick.Subscribe(n =>
                {
                    SelectToggleUI(n);

                    OnValueChanged.OnNext(n);

                }).AddTo(this);
            }

            _dropdown.onValueChanged.AddListener((n) =>
            {
                int value = 0;

                switch (_dropdownType)
                {
                    case DropdownType.Angle:
                        switch (n)
                        {
                            case 0:
                                value = 2;
                                break;
                            case 1:
                                value = 3;
                                break;
                            case 2:
                                value = 4;
                                break;

                            case 3:
                                value = 5;
                                break;
                            case 4:
                                value = 0;
                                break;
                            case 5:
                                value = 1;
                                break;
                        }
                        break;
                    case DropdownType.Color:
                        value = n;
                        break;
                }

                OnValueChanged.OnNext(value);
            });
        }

        public void SelectToggleUI(int index)
        {
            foreach (var toggleUI in _toggleUIList)
            {
                toggleUI.Select = false;
            }

            if (_toggleUIList != null && _toggleUIList.Count > 0)
            {
                _toggleUIList.Where(n => n.Index == index).First().Select = true;
            }

            if (_dropdown != null)
            {
                _dropdown.value = index;
            }
        }
    }
}