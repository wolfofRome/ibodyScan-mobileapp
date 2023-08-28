using UnityEngine;

namespace FitAndShape
{
    [CreateAssetMenu(fileName = "FitAndShapeParameter", menuName = "ScriptableObjects/CreateFitAndShapeParameter")]
    public class FitAndShapeParameter : ScriptableObject
    {
        [SerializeField] Vector2 _viewportRect;
        [SerializeField] Vector2 _measurementViewportRect;
        [SerializeField] float _fieldOfView;
        [SerializeField] float _zoomCameraPositionOffsetY;
        [SerializeField] string _apiKey;
        [SerializeField] string _csvUrl;
        [SerializeField] string _testUrl;
        [SerializeField] float _objLoadScale;
        [SerializeField] float _cameraUpperRatio;
        [SerializeField] float _cameraLowerRatio;
        [SerializeField] float _upperHeight;
        [SerializeField] float _lowerHeight;
        [SerializeField] Vector3 _cameraPosition;
        [SerializeField] float _postureWarningOffset;
        [SerializeField] string _host;

        public Vector2 ViewportRect => _viewportRect;
        public Vector2 MeasurementViewportRect => _measurementViewportRect;
        public float FieldOfView => _fieldOfView;
        public float ZoomCameraPositionOffsetY => _zoomCameraPositionOffsetY;
        public string ApiKey => _apiKey;
        public string CsvUrl => _csvUrl;
        public float ObjLoadScale => _objLoadScale;
        public Vector3 CameraPosition => _cameraPosition;
        public float PostureWarningOffset => _postureWarningOffset;
        public string Host => _host;

        public string TestUrl => _testUrl;

        public float GetCameraPositionZ(float height)
        {
            return (float)(_cameraUpperRatio - _cameraLowerRatio) * (float)(height - _lowerHeight) / (float)(_upperHeight - _lowerHeight) + _cameraLowerRatio;
            //return 8f;
        }

        public Vector3 Scale
        {
            get
            {
                Vector3 scale = Vector3.one;
                scale.x = scale.x * _objLoadScale * -1f;
                scale.y = scale.y * _objLoadScale;
                scale.z = scale.z * _objLoadScale;

                return scale;
            }
        }
    }
}