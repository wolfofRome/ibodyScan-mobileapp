using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math.EC;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace FitAndShape
{
    /// <summary>
    /// FitAndShapeのAPIをコールする時の共通クラス
    /// その処理を行う時は他のビジネスロジックではなく、必ずこのクラスを経由して呼び出しを行うこと
    /// </summary>
    public class FitAndShapeAPI : MonoBehaviour
    {
        // [重要]
        // UnityWebRequestにはデフォルトタイムアウトが存在しないので、必ずタイムアウト（秒）を設定すること
        private const int UNITY_GET_REQUEST_TIMEOUT_SEC = 8;
        private const int UNITY_POST_REQUEST_TIMEOUT_SEC = 8;
        private const int UNITY_DOWNLOAD_REQUEST_TIMEOUT_SEC = 30;

        static bool IsCanRetryError(UnityWebRequest requestResult)
        {
            if (400 <= requestResult.responseCode && requestResult.responseCode < 500)
            {
                return false; // 4xx系エラーはリトライ不可
            }

            return true;
        }

        public static async UniTask<UnityWebRequest> GetRequest(string url, string bearerToken,
            int maxRetry = 3)
        {
            int retry = 0;
            UnityWebRequest request = null;

            while (retry < maxRetry)
            {
                request = UnityWebRequest.Get(url);
                request.timeout = UNITY_GET_REQUEST_TIMEOUT_SEC;
                if (bearerToken != null)
                {
                    request.SetRequestHeader("Authorization", "bearer " + bearerToken);
                }

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    break;
                }

                if (IsCanRetryError(request) == false)
                {
                    break;
                }

                retry++;
                if (retry >= maxRetry)
                {
                    break;
                }
            }

            return request;
        }

        public static async UniTask<UnityWebRequest> PostRequest(string url, WWWForm form,
            string bearerToken, int maxRetry = 3)
        {
            int retry = 0;
            UnityWebRequest request = null;

            while (retry < maxRetry)
            {
                if (request != null)
                {
                    request.Dispose();
                    request = null;
                }

                request = UnityWebRequest.Post(url, form);
                request.timeout = UNITY_POST_REQUEST_TIMEOUT_SEC;
                request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                if (bearerToken != null)
                {
                    request.SetRequestHeader("Authorization", "bearer " + bearerToken);
                }

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    break;
                }

                if (IsCanRetryError(request) == false)
                {
                    break;
                }

                retry++;
                if (retry >= maxRetry)
                {
                    break;
                }
            }

            return request;
        }

        public static async UniTask<UnityWebRequest> PostRequestJson(string url, string userAgent, string bearerToken, 
            SerializableJson serializableJson, int maxRetry = 3)
        {
            int retry = 0;
            UnityWebRequest request = null;

            while (retry < maxRetry)
            {
                if (request != null)
                {
                    if (request.uploadHandler != null) request.uploadHandler.Dispose();
                    if (request.downloadHandler != null) request.downloadHandler.Dispose();
                    request.Dispose();
                    request = null;
                }

                request = new UnityWebRequest(url, "POST");
                request.timeout = UNITY_POST_REQUEST_TIMEOUT_SEC;
                if (serializableJson != null)
                {
                    string myjson =
                        JsonUtility.ToJson(serializableJson).Replace("\":\"\"", "\":null"); // empty String to null
                    byte[] postData = System.Text.Encoding.UTF8.GetBytes(myjson);
                    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
                }
                else
                {
                    byte[] postData = System.Text.Encoding.UTF8.GetBytes("{}");
                    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
                }

                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Accepted", "application/json");
                if (userAgent != null)
                {
                    request.SetRequestHeader("User-Agent", userAgent);
                }
                if (bearerToken != null)
                {
                    request.SetRequestHeader("Authorization", "bearer " + bearerToken);
                }

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    break;
                }

                if (IsCanRetryError(request) == false)
                {
                    break;
                }

                retry++;
                if (retry >= maxRetry)
                {
                    break;
                }
            }

            return request;
        }

        public static async UniTask<UnityWebRequest> DeleteRequest(string url, string bearerToken,
            int maxRetry = 3)
        {
            int retry = 0;
            UnityWebRequest request = null;

            while (retry < maxRetry)
            {
                request = UnityWebRequest.Delete(url);
                request.timeout = UNITY_POST_REQUEST_TIMEOUT_SEC;
                //request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                if (bearerToken != null)
                {
                    request.SetRequestHeader("Authorization", "bearer " + bearerToken);
                }

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    break;
                }

                if (IsCanRetryError(request) == false)
                {
                    break;
                }

                retry++;
                if (retry >= maxRetry)
                {
                    break;
                }
            }

            return request;
        }

        // 以下、実装の参考（Textureやダウンロード系処理）
        
        // public static async UniTask<UnityWebRequest> GetTexture(string locationUri, string resourceToken,
        //     int maxRetry = 3)
        // {
        //     // ローカルキャッシュの機能を利用したい場合、StorageUtils.GetTexture()を代わりに呼び出して下さい
        //     var uri = ECCurve.Config.RESOURCE_BASE_URL + locationUri + "?resource_token=" + resourceToken;
        //     int retry = 0;
        //     UnityWebRequest request = null;
        //     while (retry < maxRetry)
        //     {
        //         request = UnityWebRequestTexture.GetTexture(uri);
        //         request.timeout = UNITY_DOWNLOAD_REQUEST_TIMEOUT_SEC;
        //         await request.SendWebRequest();
        //
        //         if (request.result == UnityWebRequest.Result.Success)
        //         {
        //             break;
        //         }
        //
        //         if (IsCanRetryError(request) == false)
        //         {
        //             break;
        //         }
        //
        //         retry++;
        //         if (retry >= maxRetry)
        //         {
        //             break;
        //         }
        //     }
        //
        //     return request;
        // }
        //
        // public static async UniTask<UnityWebRequest> DownloadResource(string locationUri, string resourceToken,
        //     string path, int maxRetry = 3)
        // {
        //     var uri = ECCurve.Config.RESOURCE_BASE_URL + locationUri + "?resource_token=" + resourceToken;
        //     int retry = 0;
        //     UnityWebRequest request = null;
        //
        //     while (retry < maxRetry)
        //     {
        //         request = new UnityWebRequest(uri);
        //         request.timeout = UNITY_DOWNLOAD_REQUEST_TIMEOUT_SEC;
        //         request.downloadHandler = new DownloadHandlerFile(path);
        //         await request.SendWebRequest();
        //         request.downloadHandler.Dispose();
        //
        //         if (request.result == UnityWebRequest.Result.Success)
        //         {
        //             break;
        //         }
        //
        //         if (IsCanRetryError(request) == false)
        //         {
        //             break;
        //         }
        //
        //         retry++;
        //         if (retry >= maxRetry)
        //         {
        //             break;
        //         }
        //     }
        //
        //     Debug.Log($"DownloadFile completed: {path}");
        //     return request;
        // }
    }
    
    /// <summary>
    /// APIとやり取りする構造体は以下に集約する
    /// </summary>
    public abstract class SerializableJson
    {
    }

    // Request
    [Serializable]
    public class LoginRequest : SerializableJson
    {
        public string tel;
        public string password;
        public string redirect;
        public string is_demo;

        public LoginRequest(string tel, string password, string redirect, string isDemo)
        {
            this.tel = tel;
            this.password = password;
            this.redirect = redirect;
            is_demo = isDemo;
        }
    }
    
    // Response
    [Serializable]
    public class LoginResponse : SerializableJson
    {
        public LoginData data;
    }
}