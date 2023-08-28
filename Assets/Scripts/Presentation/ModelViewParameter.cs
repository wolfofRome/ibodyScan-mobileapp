using UnityEngine;

namespace Amatib
{
    [CreateAssetMenu(fileName = "ModelViewParameter", menuName = "ScriptableObjects/CreateModelViewParameter")]
    public class ModelViewParameter : ScriptableObject
    {
        [SerializeField] private float _zoomTime;
        [SerializeField] private float _pinchOutInRate;
        [SerializeField] private float _previewZoomValue;

        [SerializeField] private Vector3 _frontPosition;
        [SerializeField] private float _frontZoomValue;
        [SerializeField] private float _frontZoomTime;
        [SerializeField] private float _zoomValue;

        [SerializeField] private float _upperRatio;
        [SerializeField] private float _lowerRatio;
        [SerializeField] private float _upperHeight;
        [SerializeField] private float _lowerHeight;
        [SerializeField] private float _moveCoefficient;

        [SerializeField] private float _cameraUpperRatio;
        [SerializeField] private float _cameraLowerRatio;
        [SerializeField] float _objLoadScale;

        [SerializeField] private string _testUrl;

        public float ZoomTime => _zoomTime;
        public float PinchOutInRate => _pinchOutInRate;
        public float PreviewZoomValue => _previewZoomValue;

        public Vector3 FrontPosition => _frontPosition;
        public float FrontZoomValue => _frontZoomValue;
        public float FrontZoomTime => _frontZoomTime;
        public float ZoomValue => _zoomValue;

        public float UpperRatio => _upperRatio;
        public float LowerRatio => _lowerRatio;
        public float UpperHeight => _upperHeight;
        public float LowerHeight => _lowerHeight;
        public float MoveCoefficient => _moveCoefficient;
        public float CameraUpperRatio => _cameraUpperRatio;
        public float CameraLowerRatio => _cameraLowerRatio;
        public float ObjLoadScale => _objLoadScale;

        public string TestUrl => _testUrl;
    }
}
