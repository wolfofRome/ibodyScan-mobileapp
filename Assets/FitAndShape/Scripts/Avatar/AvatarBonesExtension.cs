using System.Collections.Generic;

namespace FitAndShape
{
    public static class AvatarBonesExtension
    {
        private static readonly Dictionary<AvatarBones, string> AvatarBonesNameMap = new Dictionary<AvatarBones, string>
        {
            { AvatarBones.Hips, "hips" }, // 恥骨
            { AvatarBones.Jushin, "Jushin" }, // 骨盤の中心？
            { AvatarBones.LeftUpLeg, "LeftUpLeg" },
            { AvatarBones.LeftDownScale1, "LeftDownScale1" },
            { AvatarBones.LeftDownScale2, "LeftDownScale2" },
            { AvatarBones.LeftGreaterTrochanter, "Daitenshi_L" }, // 大転子 -> 太ももの外側の付け根
            { AvatarBones.LeftLeg, "LeftLeg" },
            { AvatarBones.LeftPatella, "Hizagashira_L" },
            { AvatarBones.LeftFoot, "LeftFoot" },
            { AvatarBones.LeftAnkle, "Ashikubi_L" },
            { AvatarBones.LeftToe, "LeftToeBase_(1)" },
            { AvatarBones.RightUpLeg, "RightUpLeg" },
            { AvatarBones.RightDownScale1, "RightDownScale1" },
            { AvatarBones.RightDownScale2, "RightDownScale2" },
            { AvatarBones.RightGreaterTrochanter, "Daitenshi_R" },
            { AvatarBones.RightLeg, "RightLeg" },
            { AvatarBones.RightPatella, "Hizagashira_R" },
            { AvatarBones.RightFoot, "RightFoot" },
            { AvatarBones.RightAnkle, "Ashikubi_R" },
            { AvatarBones.RightToe, "RightToeBase_(1)" },
            { AvatarBones.SpineWaist, "spine" },
            { AvatarBones.Spine1, "spine1" },
            { AvatarBones.Spine2, "spine2" },
            { AvatarBones.Neck1, "neck_(1)" },
            { AvatarBones.Neck2, "neck_(2)" },
            { AvatarBones.Neck3, "neck_(3)" },
            { AvatarBones.LeftEarlobe, null },
            { AvatarBones.RightEarlobe, null },
            { AvatarBones.Head, "head" },
            { AvatarBones.LeftShoulder, "LeftShoulder" },
            { AvatarBones.LeftAcromion, null },
            { AvatarBones.LeftArm, "LeftArm" },
            { AvatarBones.LeftUpScale1, "LeftUpScale1" },
            { AvatarBones.LeftForeArm, "LeftForeArm" },
            { AvatarBones.LeftHand, "LeftHand_(1)" },
            { AvatarBones.LeftHand2, "LeftHand_(2)" },
            { AvatarBones.RightShoulder, "RightShoulder" },
            { AvatarBones.RightAcromion, null },
            { AvatarBones.RightArm, "RightArm" },
            { AvatarBones.RightUpScale1, "RightUpScale1" },
            { AvatarBones.RightForeArm, "RightForeArm" },
            { AvatarBones.RightHand, "RightHand_(1)" },
            { AvatarBones.RightHand2, "RightHand_(2)" },
            { AvatarBones.Root, "bone" },
            { AvatarBones.RightFinger1, "RightFinger1" },
            { AvatarBones.RightFinger11, "RightFinger1_1" },
            { AvatarBones.RightFinger12, "RightFinger1_2" },
            { AvatarBones.RightFinger2, "RightFinger2" },
            { AvatarBones.RightFinger21, "RightFinger2_1" },
            { AvatarBones.RightFinger22, "RightFinger2_2" },
            { AvatarBones.RightFinger3, "RightFinger3" },
            { AvatarBones.RightFinger31, "RightFinger3_1" },
            { AvatarBones.RightFinger32, "RightFinger3_2" },
            { AvatarBones.RightFinger4, "RightFinger4" },
            { AvatarBones.RightFinger41, "RightFinger4_1" },
            { AvatarBones.RightFinger42, "RightFinger4_2" },
            { AvatarBones.RightFinger5, "RightFinger5" },
            { AvatarBones.RightFinger51, "RightFinger5_1" },
            { AvatarBones.RightFinger52, "RightFinger5_2" },
            { AvatarBones.LeftFinger1, "LeftFinger1" },
            { AvatarBones.LeftFinger11, "LeftFinger1_1" },
            { AvatarBones.LeftFinger12, "LeftFinger1_2" },
            { AvatarBones.LeftFinger2, "LeftFinger2" },
            { AvatarBones.LeftFinger21, "LeftFinger2_1" },
            { AvatarBones.LeftFinger22, "LeftFinger2_2" },
            { AvatarBones.LeftFinger3, "LeftFinger3" },
            { AvatarBones.LeftFinger31, "LeftFinger3_1" },
            { AvatarBones.LeftFinger32, "LeftFinger3_2" },
            { AvatarBones.LeftFinger4, "LeftFinger4" },
            { AvatarBones.LeftFinger41, "LeftFinger4_1" },
            { AvatarBones.LeftFinger42, "LeftFinger4_2" },
            { AvatarBones.LeftFinger5, "LeftFinger5" },
            { AvatarBones.LeftFinger51, "LeftFinger5_1" },
            { AvatarBones.LeftFinger52, "LeftFinger5_2" }
        };

        public static string GetName(this AvatarBones bone)
        {
            return AvatarBonesNameMap.ContainsKey(bone) ? AvatarBonesNameMap[bone] : null;
        }
    }
}