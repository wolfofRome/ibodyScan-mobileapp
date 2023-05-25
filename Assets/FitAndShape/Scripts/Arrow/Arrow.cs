using System;
using UnityEngine;

namespace FitAndShape
{
    [Serializable]
    public sealed class Arrow
    {
        [SerializeField] MeasurementPart _measurementPart;
        [SerializeField] SpriteRenderer _spriteRenderer;

        public MeasurementPart MeasurementPart => _measurementPart;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;
    }
}
