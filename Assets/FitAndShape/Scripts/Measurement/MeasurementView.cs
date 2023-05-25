using Amatib.ObjViewer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FitAndShape
{
    public sealed class MeasurementView : MonoBehaviour
    {
        [SerializeField] MeasurementValueUI _measurementValueUIPrefab;
        [SerializeField] MeasurementValueUI _measurementWaistHistoryValueUIPrefab;
        [SerializeField] Transform _measurementValueList1Transform;
        [SerializeField] Transform _measurementValueList2Transform;
        [SerializeField] VerticalLayoutGroup _verticalLayoutGroup1;
        [SerializeField] VerticalLayoutGroup _verticalLayoutGroup2;
        [SerializeField] FitAndShapeInfoView _fitAndShapeInfoView;

        List<MeasurementValueUI> _measurementValueUIList = new List<MeasurementValueUI>();

        public IObservable<MeasurementPart> OnClick => _onClick;
        Subject<MeasurementPart> _onClick = new Subject<MeasurementPart>();

        public IObservable<MeasurementPart> OnHistoryClick => _onHistoryClick;
        Subject<MeasurementPart> _onHistoryClick = new Subject<MeasurementPart>();

#if UNITY_WEBGL_API
        readonly static int LeftCount = 10;
#endif

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetParameter(Parameter parameter)
        {
            _fitAndShapeInfoView.DateText = parameter.CreatedAt;
            _fitAndShapeInfoView.PlaceText = parameter.Place;
        }

        public void CreateMeasurementValueUI(IMeasurementCsvLoader measurementCsvLoader, int height)
        {
            int length = Enum.GetValues(typeof(MeasurementPart)).Length;

            foreach (var item in _measurementValueUIList)
            {
                Destroy(item.gameObject);
            }

            _measurementValueUIList.Clear();

            var list = MeasurementPartExtension.GetList();

            int index = 0;

            foreach (MeasurementPart measurementPart in list) 
            {
                if (!measurementPart.IsDisplay())
                {
                    continue;
                }

                MeasurementValueUI measurementValueUI = null;

#if UNITY_WEBGL_API
                switch (measurementPart)
                {
                    case MeasurementPart.WaistMaxCircumference:
                    case MeasurementPart.WaistMinCircumference:
                        measurementValueUI = Instantiate(_measurementWaistHistoryValueUIPrefab, index < LeftCount ? _measurementValueList1Transform : _measurementValueList2Transform);
                        break;
                    default:
                        measurementValueUI = Instantiate(_measurementValueUIPrefab, index < LeftCount ? _measurementValueList1Transform : _measurementValueList2Transform);
                        break;
                }
#else
                measurementValueUI = Instantiate(_measurementValueUIPrefab, _measurementValueList1Transform);

                switch (measurementPart)
                {
                    case MeasurementPart.WaistMaxCircumference:
                    case MeasurementPart.WaistMinCircumference:
                        measurementValueUI.HistoryButtonVisible = true;
                        break;
                    default:
                        measurementValueUI.HistoryButtonVisible = false;
                        break;
                }
#endif

                measurementValueUI.Initialize(measurementPart);
                measurementValueUI.Name = measurementPart.GetName();

                switch (measurementPart)
                {
                    case MeasurementPart.Hieght:
                        measurementValueUI.Value = (float)height / 10f;
                        break;
                    default:
                        measurementValueUI.Value = (float)measurementCsvLoader.GetValue(measurementPart) / 10f;
                        break;
                }

                measurementValueUI.OnClick.Subscribe(x =>
                {
                    Select(x);

                    _onClick.OnNext(x);

                }).AddTo(this);

                measurementValueUI.OnHistoryClick.Subscribe(x =>
                {
                    Select(x);

                    _onHistoryClick.OnNext(x);

                }).AddTo(this);

                _measurementValueUIList.Add(measurementValueUI);

                index++;
            }
        }

        public void Clear()
        {
            foreach (var measurementValueUI in _measurementValueUIList)
            {
                measurementValueUI.Select = false;
            }
        }

        void Select(MeasurementPart x)
        {
#if UNITY_WEBGL_API
            _verticalLayoutGroup1.enabled = false;
            _verticalLayoutGroup2.enabled = false;
#endif
            Clear();

            MeasurementValueUI activeMeasurementValueUI = _measurementValueUIList.Where(n => n.MeasurementPart == x).FirstOrDefault();

            if (activeMeasurementValueUI == null)
            {
                return;
            }

            activeMeasurementValueUI.Select = true;

#if UNITY_WEBGL_API
            activeMeasurementValueUI.transform.SetAsLastSibling();
#endif
        }
    }
}