using System.Collections.Generic;
using UnityEngine;

namespace FitAndShape
{
    public sealed class Result
    {
        private PostureVerifyPoint _point;

        public PostureVerifyPoint Point
        {
            get
            {
                return _point;
            }
            set
            {
                _point = value;
            }
        }

        private PostureCondition _condition = PostureCondition.Normal;

        public PostureCondition Condition
        {
            get
            {
                return _condition;
            }
            set
            {
                _condition = value;
            }
        }

        public RangeThreshold Threshold
        {
            get
            {
                return Point.ToRangeThreshold();
            }
        }

        public string Summary => Condition.ToSummary();

        public string Description
        {
            get
            {
                return Condition.ToDescription(DispValues.Length > 0 ? Mathf.Abs(DispValues[0]) : (float?)null, DispValues.Length > 1 ? Mathf.Abs(DispValues[1]) : (float?)null);
            }
        }

        private Vector3[] _focusPoints;

        public Vector3[] FocusPoints
        {
            get
            {
                return _focusPoints;
            }
            set
            {
                _focusPoints = value;

                var length = value.Length;
                var total = Vector3.zero;

                _scaledFocusPoints = new Vector3[length];

                for (var i = 0; i < length; i++)
                {
                    _scaledFocusPoints[i] = _focusPoints[i] * AppConst.ObjLoadScale;
                    total += _focusPoints[i];
                }

                _midFocusPoint = total / length;
            }
        }

        private Vector3 _midFocusPoint;

        public Vector3 MidFocusPoint
        {
            get
            {
                return _midFocusPoint;
            }
        }

        public Vector3 MidScaledFocusPoint
        {
            get
            {
                return _midFocusPoint * AppConst.ObjLoadScale;
            }
        }

        private float[] _rawValues = new float[] { };

        public float[] RawValues
        {
            get
            {
                return _rawValues;
            }
            set
            {
                _rawValues = value;
            }
        }

        private float[] _dispValues = new float[] { };

        public float[] DispValues
        {
            get
            {
                return _dispValues;
            }
            set
            {
                _dispValues = value;
            }
        }

        private List<Vector3[]> _scaledBaseLinePoints = new List<Vector3[]>();

        public List<Vector3[]> ScaledBaseLinePoints
        {
            get
            {
                return _scaledBaseLinePoints;
            }
        }

        private List<Vector3[]> _scaledMeasurementLinePoints = new List<Vector3[]>();

        public List<Vector3[]> ScaledMeasurementLinePoints
        {
            get
            {
                return _scaledMeasurementLinePoints;
            }
        }

        private Vector3[] _scaledFocusPoints;

        public Vector3[] ScaledFocusPoints
        {
            get
            {
                return _scaledFocusPoints;
            }
        }

        public void AddBaseLinePoints(Vector3[] linePoints)
        {
            _scaledBaseLinePoints.Add(GetScaledPoints(linePoints));
        }

        public void AddMeasurementLinePoints(Vector3[] linePoints)
        {
            _scaledMeasurementLinePoints.Add(GetScaledPoints(linePoints));
        }

        Vector3[] GetScaledPoints(Vector3[] srcPoints)
        {
            if (srcPoints == null)
            {
                return null;
            }

            var count = srcPoints.Length;

            var scaledPoints = new Vector3[count];

            for (int i = 0; i < count; i++)
            {
                scaledPoints[i] = srcPoints[i] * AppConst.ObjLoadScale;
            }

            return scaledPoints;
        }

        public override string ToString()
        {
            var dispValueText = "";

            if (DispValues != null)
            {
                foreach (var value in DispValues)
                {
                    dispValueText = string.Concat(dispValueText, value.ToString() + ",");
                }
            }

            var rawValueText = "";

            if (RawValues != null)
            {
                foreach (var value in RawValues)
                {
                    rawValueText = string.Concat(rawValueText, value.ToString() + ",");
                }
            }

            return string.Format("{0}, {1}, {2}, {3}", Point.ToRangeThreshold(), dispValueText, Point.ToDescription(), rawValueText);
        }
    }
}
