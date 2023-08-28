using System.Linq;

namespace FitAndShape
{
    public static class BonePartExtension
    {
        private static readonly BonePart[] _cartilageList = new BonePart[]
        {
            BonePart.nose_nan,
            BonePart.ear_nan,
            BonePart.neck_nan,
            BonePart.rokkotsu_nan,
            BonePart.jowan_l_nan1,
            BonePart.jowan_l_nan2,
            BonePart.jowan_r_nan1,
            BonePart.jowan_r_nan2,
            BonePart.zenwan_l_nan1,
            BonePart.zenwan_l_nan2,
            BonePart.zenwan_r_nan1,
            BonePart.zenwan_r_nan2,
            BonePart.hand_l_nan,
            BonePart.hand_r_nan,
            BonePart.kyotsui_nan,
            BonePart.yotsui_nan,
            BonePart.daitai_l_nan1,
            BonePart.daitai_l_nan2,
            BonePart.daitai_r_nan1,
            BonePart.daitai_r_nan2,
            BonePart.sune_l_nan,
            BonePart.sune_r_nan,
            BonePart.foot_l_nan,
            BonePart.foot_r_nan,
        };

        public static bool IsCartilage(this BonePart part)
        {
            return _cartilageList.Contains(part);
        }
    }
}
