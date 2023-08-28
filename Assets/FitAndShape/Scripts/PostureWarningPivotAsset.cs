using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FitAndShape
{
    [CreateAssetMenu(fileName = "PostureWarningPivotAsset", menuName = "ScriptableObjects/PostureWarningPivotAsset")]
    public class PostureWarningPivotAsset : ScriptableObject
    {
        [SerializeField] List<PostureWarningPivotEntity> _postureWarningPivotList;

        public PostureWarningPivotEntity GetEntity(Angle angle)
        {
            return _postureWarningPivotList.Where(n => n.Angle == angle).FirstOrDefault();
        }
    }
}