using System;
using UnityEngine;

namespace FitAndShape
{
    [Serializable]
    public sealed class PostureAdviceEntity
    {
        [SerializeField] PostureVerifyPoint _postureVerifyPoint;
        [SerializeField] Sprite _partSprite;
        [SerializeField] Sprite _treatmentSprite;
        [SerializeField] PostureAdvicePoint _postureAdvicePoint;

        public PostureVerifyPoint PostureVerifyPoint => _postureVerifyPoint;
        public Sprite PartSprite => _partSprite;
        public Sprite TreatmentSprite => _treatmentSprite;
        public PostureAdvicePoint PostureAdvicePoint => _postureAdvicePoint;

        public PostureAdviceEntity(PostureVerifyPoint postureVerifyPoint)
        {
            _postureVerifyPoint = postureVerifyPoint;
            _partSprite = null;
            _treatmentSprite = null;
        }
    }
}