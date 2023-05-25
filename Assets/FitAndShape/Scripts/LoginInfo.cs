using System;
using Amatib.ObjViewer.Domain;

namespace FitAndShape
{
    [Serializable]
    public sealed class LoginInfo
    {
        public readonly static string Key = "LoginInfo";

        public string UserId;
        public string Password;
        public FitAndShapeServiceType FitAndShapeServiceType;

        public static bool Exist()
        {
            var loginInfo = PlayerPrefsUtils.GetObject<LoginInfo>(Key);
            return (loginInfo != null && !string.IsNullOrEmpty(loginInfo.UserId) && !string.IsNullOrEmpty(loginInfo.Password));
        }

        public static void Clear()
        {
            PlayerPrefsUtils.SetObject(Key, new LoginInfo());
        }
    }
}