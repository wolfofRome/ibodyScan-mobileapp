using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FitAndShape
{
    public sealed class RenderTextureUpdater : MonoBehaviour
    {
        [SerializeField] List<RenderTextureController> _targetList;
        [SerializeField] Transform _parent;

        public List<RenderTextureController> TargetList => _targetList;

        Angle _preveAngle;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Initialize(float value)
        {
            foreach (RenderTextureController target in _targetList)
            {
                target.Initialize(value);
            }
        }

        public void SetViewPortRect(Vector2 value)
        {
            foreach (RenderTextureController target in _targetList)
            {
                target.TargetCamera.rect = new Rect(value.x, value.y, 1, 1);
            }
        }

        public void SetDisplayCamera(Angle angle)
        {
            foreach (RenderTextureController target in _targetList)
            {
                target.TargetCamera.gameObject.SetActive(false);
            }

            var targetList = _targetList.Where(n => n.Angle == angle);

            foreach (RenderTextureController target in targetList)
            {
                target.TargetCamera.gameObject.SetActive(true);
            }
        }

        public Camera GetDisplayCamera(Angle angle)
        {
            RenderTextureController renderTextureController = _targetList.Where(n => n.Angle == angle).FirstOrDefault();

            if (renderTextureController == null) return null;

            return renderTextureController.TargetCamera;
        }

        public void SetCameraPosition(Result result, Angle angle, FitAndShapeParameter parameter)
        {
            _preveAngle = angle;

            var renderTextureControllers = _targetList.Where(n => n.Angle == angle);

            foreach (RenderTextureController renderTextureController in renderTextureControllers)
            {
                // カメラをフォーカス位置に合わせて調整
                switch (angle)
                {
                    case Angle.Left:
                    case Angle.Right:
                    case Angle.Front:
                    case Angle.Back:
                        {
                            Vector3 position = new Vector3(0, result.MidScaledFocusPoint.y - parameter.ZoomCameraPositionOffsetY, 0);
                            renderTextureController.SetTranslate(position);
                        }
                        break;
                    case Angle.Top:
                        {
                            Vector3 position = new Vector3(0, -0.1f, 0);
                            renderTextureController.SetTranslate(position);
                        }
                        break;
                    case Angle.Under:
                    default:
                        break;
                }

                switch (angle)
                {
                    case Angle.Left:
                    case Angle.Right:
                    case Angle.Front:
                    case Angle.Back:
                        renderTextureController.SetFieldOfView(40);
                        break;
                    case Angle.Top:
                    case Angle.Under:
                        renderTextureController.SetFieldOfView(50);
                        break;
                    default:
                        break;
                }
            }
        }

        public void ResetCamera()
        {
            var renderTextureControllers = _targetList.Where(n => n.Angle == _preveAngle);

            foreach (RenderTextureController renderTextureController in renderTextureControllers)
            {
                renderTextureController.OnReset();
            }
        }
    }
}