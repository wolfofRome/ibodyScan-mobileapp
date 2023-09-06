using System.Collections.Generic;

namespace FitAndShape
{
    public static class AvatarBonesExtension
    {
        private static readonly Dictionary<AvatarBones, string> AvatarBonesNameMap = new Dictionary<AvatarBones, string>
        {
            // {AvatarBones.Hips, "hips"},
            {AvatarBones.Hips, "spine"}, // 恥骨
            {AvatarBones.Jushin, "spine"}, // 骨盤の中心？
            // {AvatarBones.Jushin, "Jushin"},
            {AvatarBones.LeftUpLeg, "thigh.L"},
            // {AvatarBones.LeftUpLeg, "LeftUpLeg"},
            {AvatarBones.LeftDownScale1, "LeftDownScale1"},
            {AvatarBones.LeftDownScale2, "LeftDownScale2"},
            {AvatarBones.LeftGreaterTrochanter, "Daitenshi_L"},　// 大転子 -> 太ももの外側の付け根
            {AvatarBones.LeftLeg, "LeftLeg"},
            {AvatarBones.LeftPatella, "Hizagashira_L"},
            {AvatarBones.LeftFoot, "LeftFoot"},
            {AvatarBones.LeftAnkle, "Ashikubi_L"},
            {AvatarBones.LeftToe, "LeftToeBase_(1)"},
            {AvatarBones.RightUpLeg, "RightUpLeg"},
            {AvatarBones.RightDownScale1, "RightDownScale1"},
            {AvatarBones.RightDownScale2, "RightDownScale2"},
            {AvatarBones.RightGreaterTrochanter, "Daitenshi_R"},
            {AvatarBones.RightLeg, "RightLeg"},
            {AvatarBones.RightPatella, "Hizagashira_R"},
            {AvatarBones.RightFoot, "RightFoot"},
            {AvatarBones.RightAnkle, "Ashikubi_R"},
            {AvatarBones.RightToe, "RightToeBase_(1)"},
            {AvatarBones.SpineWaist, "spine"},
            {AvatarBones.Spine1, "spine1"},
            {AvatarBones.Spine2, "spine2"},
            {AvatarBones.Neck1, "neck_(1)"},
            {AvatarBones.Neck2, "neck_(2)"},
            {AvatarBones.Neck3, "neck_(3)"},
            {AvatarBones.LeftEarlobe, null},
            {AvatarBones.RightEarlobe, null},
            {AvatarBones.Head, "head"},
            {AvatarBones.LeftShoulder, "LeftShoulder"},
            {AvatarBones.LeftAcromion, null},
            {AvatarBones.LeftArm, "LeftArm"},
            {AvatarBones.LeftUpScale1, "LeftUpScale1"},
            {AvatarBones.LeftForeArm, "LeftForeArm"},
            {AvatarBones.LeftHand, "LeftHand_(1)"},
            {AvatarBones.LeftHand2, "LeftHand_(2)"},
            {AvatarBones.RightShoulder, "RightShoulder"},
            {AvatarBones.RightAcromion, null},
            {AvatarBones.RightArm, "RightArm"},
            {AvatarBones.RightUpScale1, "RightUpScale1"},
            {AvatarBones.RightForeArm, "RightForeArm"},
            {AvatarBones.RightHand, "RightHand_(1)"},
            {AvatarBones.RightHand2, "RightHand_(2)"},
            {AvatarBones.Root, "bone" }
        };

        public static string GetName(this AvatarBones bone)
        {
            return AvatarBonesNameMap.ContainsKey(bone) ? AvatarBonesNameMap[bone] : null;
        }
    }
}
