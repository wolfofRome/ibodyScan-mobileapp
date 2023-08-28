using UnityEngine;

namespace FitAndShape
{
    public static class PlayerPrefsUtils
    {
        public static void SetObject<T>(string key, T obj)
        {
            var json = JsonUtility.ToJson(obj);
            PlayerPrefs.SetString(key, json);
        }

        public static T GetObject<T>(string key)
        {
            var json = PlayerPrefs.GetString(key);
            var jsonObject = JsonUtility.FromJson<T>(json);
            return jsonObject;
        }

        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        public static bool GetBool(string key)
        {
            var value = PlayerPrefs.GetInt(key, -1);
            if (value == -1)
                Debug.LogError("GetInt: cannot find value");
            return value == 1;
        }

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public static string GetString(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public static string GetString(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}