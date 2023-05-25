using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System.Linq;

namespace FitAndShape
{
    public sealed class SelectButtonGroupView : MonoBehaviour
    {
        [SerializeField] List<SelectButtonView> _selectButtonViewList;


        public void Initialize()
        {
            foreach (var item in _selectButtonViewList)
            {
                item.Initialize();
            }

            foreach (var item in _selectButtonViewList)
            {
                item.OnClick.Subscribe(_ =>
                {
                    if (!item.IsEnabled)
                    {
                        return;
                    }

                    Clear();

                    item.Active();

                }).AddTo(this);
            }
        }

        public void ShowButton(SelectButtonType selectButtonType)
        {
            Clear();

            var selectButton = _selectButtonViewList.Where(n => n.SelectButtonType == selectButtonType).FirstOrDefault();

            if (selectButton == null)
            {
                return;
            }

            selectButton.Active();
        }

        public void Clear()
        {
            foreach (var item in _selectButtonViewList)
            {
                item.NonActive();
            }
        }

        public void EnabledButton(SelectButtonType selectButtonType, bool enabled)
        {
            var selectButton = _selectButtonViewList.Where(n => n.SelectButtonType == selectButtonType).FirstOrDefault();

            if (selectButton == null)
            {
                return;
            }

            selectButton.IsEnabled = enabled;
        }
    }
}
