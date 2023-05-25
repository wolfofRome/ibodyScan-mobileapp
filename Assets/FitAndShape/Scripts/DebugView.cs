using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FitAndShape
{
    public sealed class DebugView : MonoBehaviour
    {
        [SerializeField] Button _postureButton;
        [SerializeField] Button _measurementButton;

        public IObservable<Unit> OnPosture => _onPosture;
        Subject<Unit> _onPosture = new Subject<Unit>();

        public IObservable<Unit> OnMeasurement => _onMeasurement;
        Subject<Unit> _onMeasurement = new Subject<Unit>();

        public void Initialize()
        {
            _postureButton.OnClickAsObservable().Subscribe(_ => _onPosture.OnNext(_)).AddTo(this);

            _measurementButton.OnClickAsObservable().Subscribe(_ => _onMeasurement.OnNext(_)).AddTo(this);
        }
    }
}