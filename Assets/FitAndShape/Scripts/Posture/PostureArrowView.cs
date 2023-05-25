using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FitAndShape
{
    public sealed class PostureArrowView : MonoBehaviour
    {
        [SerializeField] ImageTextHolder _imageTextHolderPrefab = default;

        List<ImageTextHolder> _diffValueList = new List<ImageTextHolder>();

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void DrawArrow(Result result, Angle angle, Camera camera)
        {
            var basePoints = result.ScaledBaseLinePoints;
            var measurementPoints = result.ScaledMeasurementLinePoints;

            int length = result.DispValues.Length;

            if (length <= 0) return;

            for (var i = 0; i < length; i++)
            {
                float dispValue = result.DispValues[i];
                ImageTextHolder imageTextHolder = Instantiate(_imageTextHolderPrefab);

                imageTextHolder.transform.SetParent(transform, false);
                imageTextHolder.text.text = string.Format("{0:0.0}" + result.Threshold.type.ToMeasurementUnit(), Mathf.Abs(dispValue));

                if (basePoints.Count > 0 && measurementPoints.Count > 0)
                {
                    Vector3 baseLastPoint = basePoints[i].Last();
                    Vector3 measurementLastPoint = measurementPoints[i].Last();
                    Vector3 p1 = camera.WorldToScreenPoint(measurementLastPoint);
                    Vector3 p2 = camera.WorldToScreenPoint(baseLastPoint);
                    Vector3 delta = p1 - p2;

                    // 90°単位の角度に変換.
                    int zDegree = Mathf.RoundToInt(Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg / 90) * 90;

                    // 膝のFTA角度の場合は矢印の向きを反転させる.
                    if (result.Point == PostureVerifyPoint.KneeJointAngle)
                    {
                        zDegree += 180;
                    }

                    imageTextHolder.image.transform.Rotate(0, 0, zDegree);
                    imageTextHolder.image.gameObject.SetActive(true);
                }

                imageTextHolder.transform.position = camera.WorldToScreenPoint(result.ScaledFocusPoints[i]);

                //if (angle == Angle.Back && (result.Condition == PostureCondition.LeftTiltPelvis || result.Condition == PostureCondition.RightTiltPelvis))
                //{
                //    diffValue.transform.Translate(transform.position + new Vector3(-120, -5, 0));
                //}
                //else
                //{
                //    diffValue.transform.Translate(transform.position + new Vector3(10, -5, 0));
                //}

                imageTextHolder.transform.SetAsLastSibling();

                _diffValueList.Add(imageTextHolder);
            }
        }

        public void Clear()
        {
            foreach (ImageTextHolder imageTextHolder in _diffValueList)
            {
                Destroy(imageTextHolder.gameObject);
            }

            _diffValueList.Clear();
        }
    }
}