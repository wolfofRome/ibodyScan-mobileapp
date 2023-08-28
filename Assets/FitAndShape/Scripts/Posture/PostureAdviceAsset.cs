using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FitAndShape
{
    [CreateAssetMenu(fileName = "PostureAdviceAsset", menuName = "ScriptableObjects/PostureAdviceAsset")]
    public sealed class PostureAdviceAsset : ScriptableObject
    {
        [SerializeField] List<PostureAdviceEntity> _postureAdviceList;

        public void Create()
        {
            _postureAdviceList = new List<PostureAdviceEntity>();

            foreach (PostureVerifyPoint postureVerifyPoint in Enum.GetValues(typeof(PostureVerifyPoint)))
            {
                _postureAdviceList.Add(new PostureAdviceEntity(postureVerifyPoint));
            }
        }

        public PostureAdviceEntity GetEntity(PostureVerifyPoint postureVerifyPoint)
        {
            return _postureAdviceList.Where(n => n.PostureVerifyPoint == postureVerifyPoint).FirstOrDefault();
        }
    }
}