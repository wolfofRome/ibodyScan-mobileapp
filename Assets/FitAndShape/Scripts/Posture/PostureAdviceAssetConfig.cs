using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FitAndShape
{
    public interface IPostureAdviceAssetConfig
    {
        MaterialColorSettings cartilageNormalMaterialSetting { get; }
        MaterialColorSettings cartilageAbnormalMaterialSetting { get; }
        MaterialColorSettings boneNormalMaterialSetting { get; }
        MaterialColorSettings boneAbnormalMaterialSetting { get; }
    }

    [CreateAssetMenu(fileName = "PostureAdviceAssetConfig", menuName = "ScriptableObjects/PostureAdviceAssetConfig")]
    public sealed class PostureAdviceAssetConfig : ScriptableObject, IPostureAdviceAssetConfig
    {
        [Serializable]
        public class TrainingAsset
        {
            [SerializeField]
            private PostureAdviceTraining _id = default;
            public PostureAdviceTraining id
            {
                get
                {
                    return _id;
                }
            }

            [SerializeField]
            private Sprite[] _images = default;
            public Sprite[] images
            {
                get
                {
                    return _images;
                }
            }
        }

        [Serializable]
        public class StretchAsset
        {
            [SerializeField]
            private PostureAdviceStretch _id = default;
            public PostureAdviceStretch id
            {
                get
                {
                    return _id;
                }
            }

            [SerializeField]
            private Sprite[] _images = default;
            public Sprite[] images
            {
                get
                {
                    return _images;
                }
            }
        }

        /// <summary>
        /// 各症状に対するトレーニングとストレッチのMAP生成用のクラス.
        /// ※詳細は下記資料参照
        ///   (Dropbox)\姿勢機能\アドバイス\症状と施術(写真割り当て).xlsx
        /// </summary>
        [Serializable]
        public class PostureConditionAsset
        {
            [SerializeField]
            private PostureCondition _id = default;
            public PostureCondition id
            {
                get
                {
                    return _id;
                }
            }

            [SerializeField]
            private PostureAdviceTraining _training = default;
            public PostureAdviceTraining training
            {
                get
                {
                    return _training;
                }
            }

            [SerializeField]
            private PostureAdviceStretch _stretch = default;
            public PostureAdviceStretch stretch
            {
                get
                {
                    return _stretch;
                }
            }
        }

        [SerializeField]
        private List<TrainingAsset> _trainingAssetList = default;

        private Dictionary<PostureAdviceTraining, TrainingAsset> _trainingAssetMap;
        private Dictionary<PostureAdviceTraining, TrainingAsset> trainingAssetMap
        {
            get
            {
                return _trainingAssetMap = _trainingAssetMap ?? _trainingAssetList.ToDictionary(x => x.id);
            }
        }

        [SerializeField]
        private List<StretchAsset> _stretchAssetList = default;

        private Dictionary<PostureAdviceStretch, StretchAsset> _stretchAssetMap;
        private Dictionary<PostureAdviceStretch, StretchAsset> stretchAssetMap
        {
            get
            {
                return _stretchAssetMap = _stretchAssetMap ?? _stretchAssetList.ToDictionary(x => x.id);
            }
        }

        [SerializeField]
        private List<PostureConditionAsset> _postureConditionAssetList = default;

        private Dictionary<PostureCondition, PostureConditionAsset> _postureConditionAssetMap;

        private Dictionary<PostureCondition, PostureConditionAsset> postureConditionAssetMap
        {
            get
            {
                return _postureConditionAssetMap = _postureConditionAssetMap ?? _postureConditionAssetList.ToDictionary(x => x.id);
            }
        }

        /// <summary>
        /// 姿勢検証における正常個所の骨のマテリアル設定.
        /// </summary>
        private MaterialColorSettings _boneNormalMaterialSetting = new MaterialColorSettings(new Color(0.7216f, 0.651f, 0.5961f), new Color(0, 0, 0));

        MaterialColorSettings IPostureAdviceAssetConfig.boneNormalMaterialSetting
        {
            get
            {
                return _boneNormalMaterialSetting;
            }
        }

        /// <summary>
        /// 姿勢検証における異常個所の骨のマテリアル設定.
        /// </summary>
        private MaterialColorSettings _boneAbnormalMaterialSetting = new MaterialColorSettings(new Color(0.8667f, 0.0275f, 0), new Color(0.7373f, 0.2588f, 0.2588f));

        MaterialColorSettings IPostureAdviceAssetConfig.boneAbnormalMaterialSetting
        {
            get
            {
                return _boneAbnormalMaterialSetting;
            }
        }

        /// <summary>
        /// 姿勢検証における正常個所の軟骨のマテリアル設定.
        /// </summary>
        private MaterialColorSettings _cartilageNormalMaterialSetting = new MaterialColorSettings(new Color(0.9843f, 0.9647f, 0.8471f), new Color(0, 0, 0));

        MaterialColorSettings IPostureAdviceAssetConfig.cartilageNormalMaterialSetting
        {
            get
            {
                return _cartilageNormalMaterialSetting;
            }
        }

        /// <summary>
        /// 姿勢検証における異常正常個所の軟骨のマテリアル設定.
        /// </summary>
        private MaterialColorSettings _cartilageAbnormalMaterialSetting = new MaterialColorSettings(new Color(1, 0, 0), new Color(0.8471f, 0.5098f, 0.5098f));

        MaterialColorSettings IPostureAdviceAssetConfig.cartilageAbnormalMaterialSetting
        {
            get
            {
                return _cartilageAbnormalMaterialSetting;
            }
        }

        /// <summary>
        /// 姿勢異常個所に関連するトレーニング画像の取得.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public TrainingAsset GetTrainingAsset(PostureCondition condition)
        {
            var postureConditionAsset = GetPostureConditionAsset(condition);
            if (postureConditionAsset != null)
            {
                TrainingAsset trainingAsset;
                return trainingAssetMap.TryGetValue(postureConditionAsset.training, out trainingAsset) ? trainingAsset : null;
            }
            return null;
        }

        /// <summary>
        /// 姿勢異常個所に関連するストレッチ画像の取得.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public StretchAsset GetStretchAsset(PostureCondition condition)
        {
            var postureConditionAsset = GetPostureConditionAsset(condition);
            if (postureConditionAsset != null)
            {
                StretchAsset stretchAsset;
                return stretchAssetMap.TryGetValue(postureConditionAsset.stretch, out stretchAsset) ? stretchAsset : null;
            }
            return null;
        }

        /// <summary>
        /// 姿勢異常個所に関連するストレッチ画像の取得.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PostureConditionAsset GetPostureConditionAsset(PostureCondition condition)
        {
            PostureConditionAsset postureConditionAsset;
            return postureConditionAssetMap.TryGetValue(condition, out postureConditionAsset) ? postureConditionAsset : null;
        }
    }
}
