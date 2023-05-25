using System;
using Amatib.ObjViewer.Domain;
using TMPro;
using UnityEngine;

namespace Amatib.ObjViewer.Presentation
{
    public class LoadingError : MonoBehaviour
    {
        // 言語設定用
        [SerializeField] private TextMeshProUGUI errorText;
        
        private void Start()
        {
            try
            {
#if UNITY_EDITOR
                const string url = "https://webgl.autotailor.jp/?key=0744CE0F-9F0F-43DD-A3F9-6C3F4EF39720&measurement_number=AT2207200598&sp=off&pointcloud=on&language=ja";
#else
                var url = Application.absoluteURL;
#endif
                var parameter = new Parameter(url);

                if (parameter.Language == Language.English)
                {
                    errorText.text = @"Failed to get data.
We apologize for the inconvenience, but please contact your system administrator.";
                }
                else
                {
                    errorText.text = @"データ取得に失敗しました。
お手数をおかけしますが、システム管理者までお問い合わせください。";
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}