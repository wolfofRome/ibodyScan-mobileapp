using System.Collections.Generic;

namespace FitAndShape
{
    public enum PostureAdvicePoint
    {
        None,
        Neck,
        Shoulder,
        Waist,
        Back,
    }

    public static class PostureAdvicePointExtension
    {
        private static readonly Dictionary<PostureAdvicePoint, string> NameMap = new Dictionary<PostureAdvicePoint, string>
        {
            {PostureAdvicePoint.Neck, "首"},
            {PostureAdvicePoint.Shoulder, "肩"},
            {PostureAdvicePoint.Waist, "腰"},
            {PostureAdvicePoint.Back, "背"}
        };

        public static string GetName(this PostureAdvicePoint value)
        {
            return NameMap.ContainsKey(value) ? NameMap[value] : null;
        }
    }
}