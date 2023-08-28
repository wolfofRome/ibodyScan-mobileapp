using System.Collections.Generic;
using System.Linq;
using Amatib.ObjViewer.Domain;
using UniRx;
using UnityEngine;

namespace FitAndShape
{
    public sealed class ServiceTypeGroupView : MonoBehaviour
    {
        [SerializeField] List<ServiceTypeView> _serviceTypeViewList;

        public void Initialize()
        {
            Clear();

            ServiceTypeView serviceTypeView = _serviceTypeViewList.Where(n => n.FitAndShapeServiceType == FitAndShapeServiceType.Distortion).First();

            serviceTypeView.Select(true);

            foreach (var item in _serviceTypeViewList)
            {
                item.OnClick.Subscribe(n =>
                {
                    Clear();

                    n.Select(true);

                }).AddTo(this);
            }
        }

        void Clear()
        {
            foreach (var item in _serviceTypeViewList)
            {
                item.Select(false);
            }
        }

        public FitAndShapeServiceType FitAndShapeServiceType
        {
            get
            {
                return _serviceTypeViewList.Where(n => n.IsSelect).First().FitAndShapeServiceType;
            }
        }
    }
}