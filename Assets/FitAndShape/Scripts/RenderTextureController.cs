using UnityEngine;

namespace FitAndShape
{
    public interface IRenderTextureController
    {
        Camera TargetCamera { get; }
        Angle Angle { get; }
        void UpdateRenderTexture();
    }

    [RequireComponent(typeof(Camera))]
    public sealed class RenderTextureController : MonoBehaviour, IRenderTextureController
    {
        [SerializeField] Camera _camera;
        [SerializeField] Angle _angle;

        public Camera TargetCamera => _camera;
        public Angle Angle => _angle;
        public Vector3 Position { get { return transform.position; } set { transform.position = value; } }

        Vector3 _prevePosition;
        float _preveFieldOfView;

        public void Initialize(float value)
        {
            _camera.fieldOfView = value;
            _prevePosition = transform.position;
            _preveFieldOfView = _camera.fieldOfView;
        }

        public void UpdateRenderTexture()
        {
            _camera.enabled = true;
        }

        public void SetTranslate(Vector3 translate)
        {
            transform.Translate(translate);
        }

        public void SetFieldOfView(float value)
        {
            _camera.fieldOfView = value;
        }

        public void OnReset()
        {
            transform.position = _prevePosition;
            _camera.fieldOfView = _preveFieldOfView;
        }
    }
}