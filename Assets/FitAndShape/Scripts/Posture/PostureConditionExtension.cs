using System.Collections.Generic;
using UnityEngine;

namespace FitAndShape
{
    public static class PostureConditionExtension
    {
        /// <summary>
        /// 姿勢判定の症状とメッセージのテーブル.
        /// </summary>
        private static readonly Dictionary<PostureCondition, Description>
            _postureConditionTable = new Dictionary<PostureCondition, Description>(){
                {PostureCondition.Normal,                   new Description("", "", "", "")},
                {PostureCondition.RightTwistNeck,new Description("", "　首のねじれ",  "首の向きがねじれています。正常な値より{0:0.0}度、右にねじれています。", "")},
                {PostureCondition.LeftTwistNeck,new Description("", "　首のねじれ",  "首の向きがねじれています。正常な値より{0:0.0}度、左にねじれています。", "")},
                {PostureCondition.RightTiltAcromion,new Description("", "　肩の曲がり（前肩, 後肩）",  "左右の肩の高さに差があります。右肩が、左肩より{0:0.0}cm下がっています。", "")},
                {PostureCondition.LeftTiltAcromion,new Description("", "　肩の曲がり（前肩, 後肩）",  "左右の肩の高さに差があります。左肩が、右肩より{0:0.0}cm下がっています。", "")},
                {PostureCondition.RightTwistWaist,new Description("", "　腰と肩のラインのねじれ",  "腰の角度を反映した線（青）と、両肩を結ぶ線がねじれています。", "")},
                {PostureCondition.RightTwistShoulder,new Description("", "　腰と肩のラインのねじれ",  "肩のラインが{0:0.0}度、右にねじれています。", "")},
                {PostureCondition.LeftTwistWaist,new Description("", "　腰と肩のラインのねじれ",  "腰の角度を反映した線（青）と、両肩を結ぶ線がねじれています。", "")},
                {PostureCondition.LeftTwistShoulder,new Description("", "　腰と肩のラインのねじれ",  "肩のラインが{0:0.0}度、左にねじれています。", "")},
                {PostureCondition.ForwardTiltKnee,new Description("", "　膝（左右のずれ）",  "両膝の位置がが前後にずれています。", "")},
                {PostureCondition.RightKneeProtruding,new Description("", "　膝（左右のずれ）",  "右膝の方が{0:0.0}cm、前に出ています。", "")},
                {PostureCondition.BackwardTiltKnee,new Description("", "　膝（左右のずれ）",  "両膝の位置がが前後にずれています。", "")},
                {PostureCondition.LeftKneeProtruding,new Description("", "　膝（左右のずれ）",  "左膝の方が{0:0.0}cm、前に出ています。", "")},
                {PostureCondition.StraightNeck,new Description("", "　ストレートネック",  "「ストレートネック」の傾向が見受けられます。本来、なだらかに後ろに湾曲しているはずの首の骨が、正常な範囲より{0:0.0}度、湾曲が足りない、もしくは前弯しています。", "")},
                {PostureCondition.NeckBackFlexion,new Description("", "　のけぞり",  "「のけぞり（頸部後屈）」の傾向が見受けられます。本来、なだらかに後ろに湾曲しているはずの首の骨が、より後ろに曲がっています。正常な範囲より{0:0.0}度、後弯し過ぎています。", "")},
                {PostureCondition.ForwardTiltHead,new Description("", "　前かがみ",  "「前かがみ」の傾向が見受けられます。耳が体の重心の軸よりも{0:0.0}cm、前に出ています。（縦に重なっているのが正常な状態です）", "")},
                {PostureCondition.BackwardTiltHead,new Description("", "　後ろかがみ",  "「後かがみ」の傾向が見受けられます。耳が体の重心の軸よりも{0:0.0}cm、後ろに出ています。（縦に重なっているのが正常な状態です）", "")},
                {PostureCondition.ForwardTiltPelvis,new Description("", "　骨盤前傾",  "「骨盤前傾」の傾向が見受けられます。正常な範囲より{0:0.0}度、前に傾き過ぎています。", "")},
                {PostureCondition.BackwardTiltPelvis,new Description("", "　骨盤後傾 ",  "「骨盤後傾」の傾向が見受けられます。正常な範囲より{0:0.0}度、後ろに傾き過ぎています。", "")},
                {PostureCondition.Stoop,new Description("", "　背骨の猫背",  "「猫背（胸椎前弯）」の傾向が見受けられます。背骨が前方に曲がっています。正常な範囲より{0:0.0}度、湾曲しすぎています。", "")},
                {PostureCondition.FlatBack,new Description("", "　背骨の平背",  "「平背」の傾向が見受けられます。背骨の前湾が足らない、もしくは後方に反り返っています。正常な範囲より{0:0.0}度、湾曲が少ないようです。", "")},
                {PostureCondition.RightTiltBody,new Description("", "「肩」の左右の高さ",  "左右の肩の高さに差があります。左肩が、右肩より{0:0.0}cm下がっています。", "")},
                {PostureCondition.LeftTiltBody,new Description("", "「肩」の左右の高さ",  "左右の肩の高さに差があります。右肩が、左肩より{0:0.0}cm下がっています。", "")},
                {PostureCondition.RightTiltPelvis,new Description("", "　骨盤（左右の高さの違い）",  "骨盤が左右に傾いています。骨盤の左側が右側よりも{0:0.0}cm下がっています。", "")},
                {PostureCondition.LeftTiltPelvis,new Description("", "　骨盤（左右の高さの違い）",  "骨盤が左右に傾いています。骨盤の右側が左側よりも{0:0.0}cm下がっています。", "")},
                {PostureCondition.RightThighShort,new Description("", "　膝の位置（太ももの長さの左右差）",  "左の大腿よりも左の大腿の長さが{0:0.0}cm、短いようです。右寛骨が左寛骨よりも後ろに傾いている可能性があります。", "")},
                {PostureCondition.LeftThighShort,new Description("", "　膝の位置（太ももの長さの左右差）",  "右の大腿よりも左の大腿の長さが{0:0.0}cm、短いようです。左寛骨が右寛骨よりも後ろに傾いている可能性があります。", "")},
                {PostureCondition.KnockKnees,new Description("", "　X脚",  "膝が内側に曲がっています。膝の外反角度の正常値より{0:0.0}度、内側に曲がっています。「X脚（外反膝）」の傾向が見受けられます。", "")},
                {PostureCondition.BowLegs,new Description("", "　O脚",  "膝が外側に曲がっています。膝の外反角度の正常値より{0:0.0}度、外側に曲がっています。「O脚（内反膝）」の傾向が見受けられます。", "")},
            };

        public static string ToSummary(this PostureCondition point)
        {
            return _postureConditionTable[point].summary;
        }

        public static string ToDescription(this PostureCondition point, float? value1, float? value2)
        {
            return string.Format(_postureConditionTable[point].detail, value1, value2);
        }
    }
}
