using System;
using UnityEngine;

namespace FitAndShape
{
    [Serializable]
    public sealed class CommentEntity
    {
        [SerializeField] string name;
        [SerializeField] string comment;

        public string Name => name;
        public string Comment => comment;

        PostureVerifyPoint _postureVerifyPoint;

        public PostureVerifyPoint PostureVerifyPoint => _postureVerifyPoint;

        public void SetPostureVerifyPoint()
        {
            foreach (PostureVerifyPoint value in Enum.GetValues(typeof(PostureVerifyPoint)))
            {
                string name = Enum.GetName(typeof(PostureVerifyPoint), value);

                if (name.Contains(this.name, StringComparison.OrdinalIgnoreCase))
                {
                    _postureVerifyPoint = value;
                    break;
                }
            }
        }

        public override string ToString()
        {
            return $"Name:{name}, PostureVerifyPoint:{_postureVerifyPoint}, Comment:{comment}";
        }
    }
}