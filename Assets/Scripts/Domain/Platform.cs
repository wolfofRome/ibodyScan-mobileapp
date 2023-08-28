using System;

namespace Amatib.ObjViewer.Domain
{
    public enum Platform
    {
        /// <summary>
        /// スマートフォン
        /// </summary>
        Sp,
        /// <summary>
        /// パーソナルコンピュータ
        /// </summary>
        Pc
    }
    
    public static class PlatformExtensions
    {
        public static Platform FromQueryParameter(string parameter)
        {
            return "on".Equals(parameter, StringComparison.OrdinalIgnoreCase) ? Platform.Sp : Platform.Pc;
        }
    }
}