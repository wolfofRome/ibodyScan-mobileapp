using System;
using UnityEngine;

namespace FitAndShape
{
    [Serializable]
    public class RangeThreshold
    {
        [SerializeField]
        private float _min = default;
        public float min
        {
            get
            {
                return _min;
            }
        }

        [SerializeField]
        private float _max = default;
        public float max
        {
            get
            {
                return _max;
            }
        }

        [SerializeField]
        private VerifyType _type = default;
        public VerifyType type
        {
            get
            {
                return _type;
            }
        }

        [SerializeField]
        private bool _containsMinMax = false;
        public bool containsMinMax
        {
            get
            {
                return _containsMinMax;
            }
        }

        public RangeThreshold(float min, float max, VerifyType type, bool containsMinMax = false)
        {
            _min = (type == VerifyType.Angle ? Mathf.DeltaAngle(0, min) : min);
            _max = (type == VerifyType.Angle ? Mathf.DeltaAngle(0, max) : max);
            _type = type;
            _containsMinMax = containsMinMax;
        }

        public float Diff(float src)
        {
            var diff = 0f;

            src = (type == VerifyType.Angle ? Mathf.DeltaAngle(0, src) : src);

            var compareResult = CompareTo(src);
            if (compareResult != 0)
            {
                diff = compareResult > 0 ? src - max : src - min;
            }
            return diff;
        }

        public int CompareTo(float src)
        {
            var value = (type == VerifyType.Angle ? Mathf.DeltaAngle(0, src) : src);
            return Contains(value) ? 0 : (value < min || Mathf.Approximately(value, min) ? -1 : 1);
        }

        public bool Contains(float src)
        {
            var value = (type == VerifyType.Angle ? Mathf.DeltaAngle(0, src) : src);
            return (containsMinMax && Approximately(value)) || (min < value && value < max);
        }

        private bool Approximately(float src)
        {
            return Mathf.Approximately(src, min) || Mathf.Approximately(src, max);
        }

        public override string ToString()
        {
            return string.Format("{0,7}, {1,10}, {2,10}", type.ToString(), min, max);
        }
    }

}
