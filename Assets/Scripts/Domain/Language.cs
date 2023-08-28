namespace Amatib.ObjViewer.Domain
{
    public enum Language
    {
        English,
        Japanese
    }
    
    public static class LanguageExtensions
    {
        public static Language FromQueryParameter(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter)) return Language.Japanese;

            return parameter.ToLower() switch
            {
                "ja" => Language.Japanese,
                "en" => Language.English,
                _ => Language.Japanese
            };
        }
    }
}