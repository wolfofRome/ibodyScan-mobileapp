using System;
using UnityEngine;

namespace FitAndShape
{
    [Serializable]
    public sealed class BodyDistortionsEntity : IEquatable<BodyDistortionsEntity>
    {
        [SerializeField] string name;
        [SerializeField] string condition;

        public BodyDistortionsEntity(PostureVerifyPoint postureVerifyPoint, PostureCondition postureCondition)
        {
            name = postureVerifyPoint.ToString();
            condition = postureCondition.ToString();
        }

        public bool Equals(BodyDistortionsEntity other)
        {
            return other.name == name && other.condition == condition;
        }
    }
}