using System.Collections.Generic;

namespace FitAndShape
{
    public static class PostureVerifyPointExtension
    {
        private static readonly Dictionary<PostureVerifyPoint, PostureInfo> _postureInfoTable = new Dictionary<PostureVerifyPoint, PostureInfo>()
        {
            // 左右アングル.
            { PostureVerifyPoint.UpperInclinationRatio,
                new PostureInfo(
                    new RangeThreshold(-1.1f, 1.1f, VerifyType.Length),
                "重心Y軸ラインと耳の乖離",
                    new BonePart[] {
                        BonePart.head_b,
                        BonePart.neck_b,
                        BonePart.kyotsui_b,
                        BonePart.yotsui_b,
                        BonePart.nose_nan,
                        BonePart.ear_nan,
                        BonePart.neck_nan,
                        BonePart.kyotsui_nan,
                        BonePart.yotsui_nan
                    }) },
            { PostureVerifyPoint.LowerInclinationRatio,
                new PostureInfo(
                    new RangeThreshold(-0.5f, 0.5f, VerifyType.Length),
                    "重心Y軸ラインとヒザの乖離",
                    new BonePart[] {
                        BonePart.daitai_l_b,
                        BonePart.daitai_r_b,
                        BonePart.sune_l_b,
                        BonePart.sune_r_b,
                        BonePart.daitai_l_nan1,
                        BonePart.daitai_l_nan2,
                        BonePart.daitai_r_nan1,
                        BonePart.daitai_r_nan2,
                        BonePart.sune_l_nan,
                        BonePart.sune_r_nan
                    }) },
            { PostureVerifyPoint.PelvisInclinationAngle,
                new PostureInfo(
                    new RangeThreshold(-3, 3, VerifyType.Angle),
                "骨盤の前後傾斜角度",
                    new BonePart[] {
                        BonePart.senkotsu_b,
                        BonePart.kankotsu_b
                    }) },
            { PostureVerifyPoint.ThoracicVertebraAngle,
                new PostureInfo(
                    new RangeThreshold(-3, 3, VerifyType.Angle),
                    "背骨(胸椎)角度の正常値からの誤差",
                    new BonePart[] {
                        BonePart.kyotsui_b,
                        BonePart.kyotsui_nan,
                    }) },
            { PostureVerifyPoint.LumbarVertebraAngle,
                new PostureInfo(
                    new RangeThreshold(-3, 3, VerifyType.Angle),
                    "背骨(腰椎上部)角度の正常値からの誤差",
                    new BonePart[] {
                        BonePart.yotsui_b,
                        BonePart.yotsui_nan,
                    }) },
            { PostureVerifyPoint.NeckCurveAngle,
                new PostureInfo(
                    new RangeThreshold(-3, 3, VerifyType.Angle),
                    "首の湾曲角度の正常値からの誤差",
                    new BonePart[] {
                        BonePart.neck_b,
                        BonePart.neck_nan,
                    }) },
            
            // 前面アングル.
            { PostureVerifyPoint.BodyInclinationRatio,
                new PostureInfo(
                    new RangeThreshold(-0.8f, 0.8f, VerifyType.Length),
                    "重心Y軸ラインと頭頂との乖離",
                    new BonePart[] {
                        BonePart.head_b,
                        BonePart.neck_b,
                        BonePart.kyotsui_b,
                        BonePart.yotsui_b,
                    }) },
            { PostureVerifyPoint.UpperLegLengthRatio,
                new PostureInfo(
                    new RangeThreshold(-0.8f, 0.8f, VerifyType.Length),
                    "大腿骨大転子から膝頭までの両足間の差 （右ー左）",
                    new BonePart[] {
                        BonePart.kankotsu_b
                    }) },
            { PostureVerifyPoint.LowerLegLengthRatio,
                new PostureInfo(
                    new RangeThreshold(-0.5f, 0.5f, VerifyType.Length),
                    "膝頭から足首までの高さの両足間の差",
                    new BonePart[] {
                        BonePart.sune_l_b,
                        BonePart.sune_r_b,
                        BonePart.sune_l_nan,
                        BonePart.sune_r_nan
                    }) },
            { PostureVerifyPoint.ShoulderInclinationRatio,
                new PostureInfo(
                    new RangeThreshold(-0.5f, 0.5f, VerifyType.Length),
                    "肩峰の左右の高さの差（右ー左）",
                    new BonePart[] {
                        BonePart.sakotsu_l_b,
                        BonePart.sakotsu_r_b
                    }) },
            { PostureVerifyPoint.PelvisInclinationRatio,
                new PostureInfo(
                    new RangeThreshold(-0.5f, 0.5f, VerifyType.Length),
                    "左右の上前腸骨棘の高さの差（右ー左）",

                    new BonePart[] {
                        BonePart.sakotsu_l_b,
                        BonePart.sakotsu_r_b
                    }) },
            { PostureVerifyPoint.KneeJointAngle,
                new PostureInfo(
                    new RangeThreshold(174, 178, VerifyType.Angle, true),
                    "ヒザ関節の外反角度（FTA）",
                    new BonePart[] {
                        BonePart.daitai_l_b,
                        BonePart.sune_l_b,
                        BonePart.daitai_r_b,
                        BonePart.sune_r_b,
                        BonePart.daitai_l_nan1,
                        BonePart.daitai_l_nan2,
                        BonePart.sune_l_nan,
                        BonePart.daitai_r_nan1,
                        BonePart.daitai_r_nan2,
                        BonePart.sune_r_nan
                    }) },
            
            // 上アングル.
            { PostureVerifyPoint.ShoulderDistortionAngle,
                new PostureInfo(
                    new RangeThreshold(-2, 2, VerifyType.Angle),
                    "左右の鎖骨角度の正常角度からの差",
                    new BonePart[] {
                        BonePart.sakotsu_l_b,
                        BonePart.sakotsu_r_b,
                        BonePart.kenko_l_b,
                        BonePart.kenko_r_b
                    }) },
            { PostureVerifyPoint.NeckDistortionAngle,
                new PostureInfo(
                    new RangeThreshold(-3, 3, VerifyType.Angle),
                    "首の左右回転角度",
                    new BonePart[] {
                        BonePart.head_b,
                        BonePart.nose_nan,
                        BonePart.ear_nan
                    }) },
            
            // 下アングル.
            { PostureVerifyPoint.UpperBodyDistortionAngle,
                new PostureInfo(
                    new RangeThreshold(-3, 3, VerifyType.Angle),
                    "腰の角度を反映した線（青）と、左右の肩を結ぶ線の角度差",
                    new BonePart[] {
                        BonePart.sakotsu_l_b,
                        BonePart.sakotsu_r_b,
                        BonePart.kenko_l_b,
                        BonePart.kenko_r_b
                    }) },
            { PostureVerifyPoint.KneeAnteroposteriorDiffRatio,
                new PostureInfo(
                    new RangeThreshold(-0.5f, 0.5f, VerifyType.Length),
                    "膝頭の前後差（右ー左）",
                    new BonePart[] {
                        BonePart.daitai_l_b,
                        BonePart.daitai_r_b,
                        BonePart.sune_l_b,
                        BonePart.sune_r_b,
                        BonePart.daitai_l_nan1,
                        BonePart.daitai_l_nan2,
                        BonePart.daitai_r_nan1,
                        BonePart.daitai_r_nan2,
                        BonePart.sune_l_nan,
                        BonePart.sune_r_nan
                    }) },
        };

        public static RangeThreshold ToRangeThreshold(this PostureVerifyPoint point)
        {
            return _postureInfoTable[point].threshold;
        }

        public static string ToDescription(this PostureVerifyPoint point)
        {
            return _postureInfoTable[point].description;
        }

        public static BonePart[] GetBoneParts(this PostureVerifyPoint point)
        {
            return _postureInfoTable[point].boneParts;
        }
    }
}
