using System;
using UnityEngine;
using UniRx;

namespace Amatib.ObjViewer.Presentation
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private float minFieldOfView;
        [SerializeField] private float maxFieldOfView;
        [SerializeField] private Camera _mainCamera;

        private IDisposable _disposable = null;
        private bool _isZoom = false;

        public float Zoom
        {
            get => _mainCamera.fieldOfView;
            set => _mainCamera.fieldOfView = Mathf.Clamp(value, minFieldOfView, maxFieldOfView);
        }

        public void OnZoom(float value, float zoomTime)
        {
            if (_isZoom) return;

            float fromValue = Zoom;
            float toValue = Zoom + value;

            _isZoom = true;

            _disposable?.Dispose();
            _disposable = null;

            _disposable = Observable.EveryUpdate().Select(_ => Time.deltaTime).Scan((total, current) => total + current).TakeWhile(n => n < zoomTime).Subscribe(n =>
            {
                Zoom = Mathf.Lerp(fromValue, toValue, n / zoomTime);

            }, () => 
            {
                Zoom = toValue;
                _isZoom = false;
            });
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}