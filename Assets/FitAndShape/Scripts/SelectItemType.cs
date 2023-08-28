using System.Collections.Generic;

namespace FitAndShape
{
    public enum SelectType
    {
        Angle,
        Color,
    }

    public enum SelectItemType
    {
        Color,
        Monochrome,
        Left,
        Right,
        Front,
        Back,
        Top,
        Under
    }

    public static class SelectItemTypeExtension
    {
        private static readonly Dictionary<SelectItemType, string>
            _table = new Dictionary<SelectItemType, string>(){
                { SelectItemType.Color, "カラー"},
                { SelectItemType.Monochrome, "モノクロ"},
                { SelectItemType.Left, "左"},
                { SelectItemType.Right, "右"},
                { SelectItemType.Front, "正面"},
                { SelectItemType.Back, "背面"},
                { SelectItemType.Top, "上"},
                { SelectItemType.Under, "下"}
            };

        public static string ToName(this SelectItemType value)
        {
            return _table[value];
        }

        public static SelectType ToSelectType(this SelectItemType value)
        {
            switch (value)
            {
                case SelectItemType.Color:
                case SelectItemType.Monochrome:
                    return SelectType.Color;
                default:
                    return SelectType.Angle;
            }
        }

        public static ColorType ToColorType(this SelectItemType value)
        {
            switch (value)
            {
                case SelectItemType.Color:
                    return ColorType.Color;
                case SelectItemType.Monochrome:
                    return ColorType.Monochrome;
                default:
                    return ColorType.Color;
            }
        }

        public static Angle ToAngle(this SelectItemType value)
        {
            switch (value)
            {
                case SelectItemType.Top:
                    return Angle.Top;
                case SelectItemType.Under:
                    return Angle.Under;
                case SelectItemType.Left:
                    return Angle.Left;
                case SelectItemType.Right:
                    return Angle.Right;
                case SelectItemType.Front:
                    return Angle.Front;
                case SelectItemType.Back:
                    return Angle.Back;
                default:
                    return Angle.Front;
            }
        }
    }
}