using System;
using UnityEngine;

namespace FitAndShape
{
    [Serializable]
    public sealed class LoginData
    {
        public readonly static string Key = "LoginData";

        [SerializeField] string fitandshape_user_session;
        [SerializeField] string customer_id;
        [SerializeField] string gender;
        [SerializeField] string measurement_number;
        [SerializeField] string place;
        [SerializeField] string created_at;
        [SerializeField] string token;
        [SerializeField] int height;
        [SerializeField] private int lastUpdatedAt;

        public string UserSession => fitandshape_user_session;
        public string CustomerID => customer_id;
        public string Gnder => gender;
        public string MeasurementNumber => measurement_number;
        public string Place => place;
        public string CreatedAt => CreatedAtExtension.Format(created_at);
        public string Token => token;
        public int Height => height;

        public int LastUpdatedAt => lastUpdatedAt;

        public void SetMeasurementNumber(string value)
        {
            measurement_number = value;
        }

        public void SetLastUpdatedAt(int value)
        {
            lastUpdatedAt = value;
        }

        public override string ToString()
        {
            return $"UserSession:{UserSession}, CustomerID:{CustomerID}, Gnder:{Gnder}, MeasurementNumber:{MeasurementNumber}, Place:{Place}, CreatedAt:{CreatedAt}, Token:{Token}, Height:{Height}";
        }

        public bool IsDemo => measurement_number == FSConstants.DEMO_MEASUREMENT_NUMBER;

        public static bool Exist()
        {
            LoginData loginInfo = PlayerPrefsUtils.GetObject<LoginData>(Key);
            return (loginInfo != null && !string.IsNullOrEmpty(loginInfo.UserSession));
        }

        public static void Clear()
        {
            PlayerPrefsUtils.SetObject(Key, new LoginData());
        }
    }
}