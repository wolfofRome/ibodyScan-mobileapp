using System.Collections.Generic;

namespace FitAndShape
{
    public static class MeasurementPartExtension
    {
        private static readonly Dictionary<MeasurementPart, string> NameMap = new Dictionary<MeasurementPart, string>
        {
            {MeasurementPart.Hieght, "身長"},
            {MeasurementPart.BackShoulderWidth, "背肩幅"},
            {MeasurementPart.RightArmLength, "腕長（右)"},
            {MeasurementPart.LeftArmLength, "腕長（左)"},
            {MeasurementPart.FirstLumbarHeight, "第一腰椎高"},
            {MeasurementPart.InseamHeight, "股下高"},
            {MeasurementPart.NeckCircumference, "ネック"},
            {MeasurementPart.RightShoulder, "肩（右）"},
            {MeasurementPart.LeftShoulder, "肩（左）"},
            {MeasurementPart.ChestCircumference, "チェスト"},
            {MeasurementPart.BustTopCircumference, "バストトップ"},
            {MeasurementPart.WaistMaxCircumference, "ウエスト(最大)"},
            {MeasurementPart.WaistMinCircumference, "ウエスト(最小)"},
            {MeasurementPart.Hip1Circumference, "ヒップ①"},
            {MeasurementPart.Hip2Circumference, "ヒップ②"},
            {MeasurementPart.Hip3Circumference, "ヒップ③"},
            {MeasurementPart.Hip4Circumference, "ヒップ④"},
            {MeasurementPart.Hip5Circumference, "ヒップ⑤"},
            {MeasurementPart.RightUpperArmCircumference, "二の腕(右)"},
            {MeasurementPart.LeftUpperArmCircumference, "二の腕(左)"},
            {MeasurementPart.RightThighCircumference, "太もも(右)"},
            {MeasurementPart.LeftThighCircumference, "太もも(左)"},
            {MeasurementPart.RightLowerLegMaxCircumference, "ふくらはぎ(右)"},
            {MeasurementPart.LeftLowerLegMaxCircumference, "ふくらはぎ(左)"},
            {MeasurementPart.RightLowerLegMinCircumference, "足首(右)"},
            {MeasurementPart.LeftLowerLegMinCircumference, "足首(左)"},
            {MeasurementPart.RightWristCircumference, "手首(右)"},
            {MeasurementPart.LeftWristCircumference, "手首(左)"},
        };

        public static string GetName(this MeasurementPart measurementPart)
        {
            return NameMap.ContainsKey(measurementPart) ? NameMap[measurementPart] : null;
        }

        public static bool IsDisplay(this MeasurementPart measurementPart)
        {
            switch (measurementPart)
            {
                case MeasurementPart.BackShoulderWidth:
                case MeasurementPart.RightShoulder:
                case MeasurementPart.LeftShoulder:
                case MeasurementPart.RightArmLength:
                case MeasurementPart.LeftArmLength:
                case MeasurementPart.InseamHeight:
                    return false;
                default: return true;
            }
        }

        public static List<MeasurementPart> GetList()
        {
            List<MeasurementPart> list = new List<MeasurementPart>();

            list.Add(MeasurementPart.Hieght);
            list.Add(MeasurementPart.NeckCircumference);
            list.Add(MeasurementPart.ChestCircumference);
            list.Add(MeasurementPart.BustTopCircumference);
            list.Add(MeasurementPart.WaistMaxCircumference);
            list.Add(MeasurementPart.WaistMinCircumference);
            list.Add(MeasurementPart.RightUpperArmCircumference);
            list.Add(MeasurementPart.LeftUpperArmCircumference);
            list.Add(MeasurementPart.RightWristCircumference);
            list.Add(MeasurementPart.LeftWristCircumference);
            list.Add(MeasurementPart.FirstLumbarHeight);
            list.Add(MeasurementPart.Hip1Circumference);
            list.Add(MeasurementPart.Hip2Circumference);
            list.Add(MeasurementPart.Hip3Circumference);
            list.Add(MeasurementPart.Hip4Circumference);
            list.Add(MeasurementPart.Hip5Circumference);
            list.Add(MeasurementPart.RightThighCircumference);
            list.Add(MeasurementPart.LeftThighCircumference);
            list.Add(MeasurementPart.RightLowerLegMaxCircumference);
            list.Add(MeasurementPart.LeftLowerLegMaxCircumference);
            list.Add(MeasurementPart.RightLowerLegMinCircumference);
            list.Add(MeasurementPart.LeftLowerLegMinCircumference);

            return list;
        }
    }
}