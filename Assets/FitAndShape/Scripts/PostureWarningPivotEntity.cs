using System;
using UnityEngine;

namespace FitAndShape
{
    [Serializable]
    public sealed class PostureWarningPivotEntity
    {
        [SerializeField] Angle _angle;
        [SerializeField] Vector2 _pivot;

        public Angle Angle => _angle;
        public Vector2 Pivot => _pivot;
    }
}