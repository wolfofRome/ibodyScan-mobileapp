using System.Collections.Generic;

namespace FitAndShape
{
    public enum PostureAdviceType
    {
        None,
        Part,
        Treatment,
    }

    public static class PostureAdviceTypeExtension
    {
        private static readonly Dictionary<PostureAdviceType, string> NameMap = new Dictionary<PostureAdviceType, string>
        {
            {PostureAdviceType.Part, "痛みや凝りが出やすい部位"},
            {PostureAdviceType.Treatment, "施術例"}
        };

        public static string GetName(this PostureAdviceType value)
        {
            return NameMap.ContainsKey(value) ? NameMap[value] : null;
        }
    }
}