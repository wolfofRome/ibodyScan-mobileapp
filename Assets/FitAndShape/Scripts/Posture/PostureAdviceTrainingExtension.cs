using System.Collections.Generic;
using UnityEngine;

namespace FitAndShape
{
    public static class PostureAdviceTrainingExtension
    {
        /// <summary>
        /// アドバイス用トレーニングのテーブル.
        /// </summary>
        private static readonly Dictionary<PostureAdviceTraining, TrainingInfo>
            _trainingInfoTable = new Dictionary<PostureAdviceTraining, TrainingInfo>(){
                // TODO:説明文は展開して頂き次第、差し替える.
                { PostureAdviceTraining.Cat,                                        new TrainingInfo("CAT運動", "")},
                { PostureAdviceTraining.CrossCrunchLeftHandRightKnee,               new TrainingInfo("Cross Crunch", "")},
                { PostureAdviceTraining.CrossCrunchRightHandLeftKnee,               new TrainingInfo("Cross Crunch", "")},
                { PostureAdviceTraining.HipAbductorMuscleLeft,                      new TrainingInfo("左足股関節外転筋エクササイズ", "")},
                { PostureAdviceTraining.HipAbductorMuscleRight,                     new TrainingInfo("右足股関節外転筋エクササイズ", "")},
                { PostureAdviceTraining.HipJointInternalRotation,                   new TrainingInfo("股関節内転エクササイズ", "")},
                { PostureAdviceTraining.KneeToChest,                                new TrainingInfo("Knee to Chest", "")},
                { PostureAdviceTraining.KneeToElbowLeftHandRightKnee,               new TrainingInfo("Knee to Elbow（左手右膝）", "")},
                { PostureAdviceTraining.KneeToElbowRightHandLeftKnee,               new TrainingInfo("Knee to elbow（右手左膝）", "")},
                { PostureAdviceTraining.SideBendingMidlineLeft,                     new TrainingInfo("側屈・左中殿筋トレーニング", "")},
                { PostureAdviceTraining.SideBendingMidlineRight,                    new TrainingInfo("側屈・右中殿筋トレーニング", "")},
                { PostureAdviceTraining.SitUp,                                      new TrainingInfo("Sit up", "")},
                { PostureAdviceTraining.TubeOpening,                                new TrainingInfo("チューブ開き運動", "")},
                { PostureAdviceTraining.TubeShoulderRotation,                       new TrainingInfo("肩回し運動（チューブ）", "")},
                { PostureAdviceTraining.KneeToElbowCrossCrunchLeftHandRightKnee,    new TrainingInfo("Knee to Elbow（CrossCrunch）", "")},
                { PostureAdviceTraining.KneeToElbowCrossCrunchRightHandLeftKnee,    new TrainingInfo("Knee to Elbow（CrossCrunch）", "")},
                { PostureAdviceTraining.HipAbductorMuscle,                          new TrainingInfo("股関節外転エクササイズ", "")},
            },
            _trainingInfoTableCHN = new Dictionary<PostureAdviceTraining, TrainingInfo>(){
                { PostureAdviceTraining.Cat,                                        new TrainingInfo("中国式CAT運動", "")},
                { PostureAdviceTraining.CrossCrunchLeftHandRightKnee,               new TrainingInfo("中国式Cross Crunch", "")},
                { PostureAdviceTraining.CrossCrunchRightHandLeftKnee,               new TrainingInfo("中国式Cross Crunch", "")},
                { PostureAdviceTraining.HipAbductorMuscleLeft,                      new TrainingInfo("中国式左足股関節外転筋エクササイズ", "")},
                { PostureAdviceTraining.HipAbductorMuscleRight,                     new TrainingInfo("中国式右足股関節外転筋エクササイズ", "")},
                { PostureAdviceTraining.HipJointInternalRotation,                   new TrainingInfo("中国式股関節内転エクササイズ", "")},
                { PostureAdviceTraining.KneeToChest,                                new TrainingInfo("中国式Knee to Chest", "")},
                { PostureAdviceTraining.KneeToElbowLeftHandRightKnee,               new TrainingInfo("中国式Knee to Elbow（左手右膝）", "")},
                { PostureAdviceTraining.KneeToElbowRightHandLeftKnee,               new TrainingInfo("中国式Knee to elbow（右手左膝）", "")},
                { PostureAdviceTraining.SideBendingMidlineLeft,                     new TrainingInfo("中国式側屈・左中殿筋トレーニング", "")},
                { PostureAdviceTraining.SideBendingMidlineRight,                    new TrainingInfo("中国式側屈・右中殿筋トレーニング", "")},
                { PostureAdviceTraining.SitUp,                                      new TrainingInfo("中国式Sit up", "")},
                { PostureAdviceTraining.TubeOpening,                                new TrainingInfo("中国式チューブ開き運動", "")},
                { PostureAdviceTraining.TubeShoulderRotation,                       new TrainingInfo("中国式肩回し運動（チューブ）", "")},
                { PostureAdviceTraining.KneeToElbowCrossCrunchLeftHandRightKnee,    new TrainingInfo("中国式Knee to Elbow（CrossCrunch）", "")},
                { PostureAdviceTraining.KneeToElbowCrossCrunchRightHandLeftKnee,    new TrainingInfo("中国式Knee to Elbow（CrossCrunch）", "")},
                { PostureAdviceTraining.HipAbductorMuscle,                          new TrainingInfo("中国式股関節外転エクササイズ", "")},
            };

        public static string ToName(this PostureAdviceTraining training)
        {
            switch (PlayerPrefs.GetString("Lang"))
            {
                case "Japanese":
                default:
                    return _trainingInfoTable[training].name;
                case "Chinese":
                    return _trainingInfoTableCHN[training].name;
            }
        }

        public static string ToDescription(this PostureAdviceTraining training)
        {
            switch (PlayerPrefs.GetString("Lang"))
            {
                case "Japanese":
                default:
                    return _trainingInfoTable[training].description;
                case "Chinese":
                    return _trainingInfoTable[training].description;
            }
        }
    }
}
