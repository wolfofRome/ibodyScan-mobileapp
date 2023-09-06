using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Amatib.ObjViewer.Domain;
using BestHTTP;
using Cysharp.Threading.Tasks;
using FitAndShape;
using UnityEngine;

/// <summary>
/// 認証情報等の管理をこちらで行います。
/// </summary>
namespace FitAndShape
{
    /// <summary>
    /// 認証系はここに全て集約します
    /// </summary>
    public class AuthManager : SingletonMonoBehaviour<AuthManager>
    {
        // TODO: 本当はここでログインデータとか保持して勝手にPlayerPrefsを触らないようなルールにしたい
        // 例えばユーザーデータを取得するタイミングで「最後にリフレッシュした時刻」を比較して、n分経過していたら再度ユーザーデータの取得を行う、等…
        // タイマー系（コルーチンとかUniTaskとか）はリファクタリングが終わるまで保留にしておいて、最終取得時の時間比較でやる（一旦30min）
        private LoginData _loginData; // 現在のログイン情報

        private float lastUpdatedAt;

        private string _userAgent;


        // TODO: 定数クラスに移動
        readonly static string LOGIN_URL = "https://api.fit-shape.jp/api/app/login";
        readonly static string DEMO_LOGIN_URL = "https://api.fit-shape.jp/api/app/demo-login";
        private readonly static int AUTH_REFRESH_RATE_SECONDS = 90 * 60; 

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        }

        /// <summary>
        /// この処理でログインした場合、IDとパスワードを自動保存する
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="tel"></param>
        /// <param name="password"></param>
        /// <param name="ct"></param>
        public async UniTask LoginWithInput(FitAndShapeServiceType serviceType, string tel, string password, CancellationToken ct)
        {
            LoginResponse response = await SendLoginRequest(tel, password, ct);
            LoginInfo loginInfo = new LoginInfo();
            loginInfo.FitAndShapeServiceType = serviceType;
            loginInfo.UserId = tel;
            loginInfo.Password = password;
            SetLoginInfo(loginInfo);
            SetLoginData(response.data);
        }

        public async UniTask LoginAsDemo(FitAndShapeServiceType serviceType, CancellationToken ct)
        {
            LoginResponse response = await SendLoginRequestDemo(ct);
            LoginInfo loginInfo = new LoginInfo();
            loginInfo.FitAndShapeServiceType = serviceType;
            SetLoginInfo(loginInfo);
            SetLoginData(response.data);
        }

        /// <summary>
        /// 自動ログインの実行
        /// </summary>
        /// <param name="userAgent"></param>
        public async UniTask AutoLogin(CancellationToken ct)
        {
            Debug.Log("[AutoLogin]: start");
            LoginInfo loginInfo = GetLoginInfo();
            try
            {
                LoginResponse loginResponse = await SendLoginRequest(loginInfo.UserId, loginInfo.Password, ct);
                SetLoginData(loginResponse.data);
                Debug.Log("[AutoLogin]: succeed");
            }
            catch (Exception ex)
            {
                // ログイン時にエラーが発生しますが、ここで例外は処理しません
                Debug.Log("[AutoLogin]: failed");
                throw ex;
            }
        }

        /// <summary>
        /// デモログイン
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async UniTask<LoginResponse> SendLoginRequestDemo(CancellationToken ct)
        {
            //TODO: CancellationToken実装
            using var request = await FitAndShapeAPI.PostRequestJson(DEMO_LOGIN_URL, GetUserAgent(), null,
                new LoginRequest(null, null, "body-distortion", "true"));
            if (request.responseCode != 200)
            {
                throw new Exception(
                    $"Login Error. StatusCode:{request.responseCode}, Message:{request.error}");
            }

            LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            return loginResponse;
        }

        public async UniTask<LoginResponse> SendLoginRequest(string phoneNumber, string password,
            CancellationToken token)
        {
            // TODO: cancellationTokenの未使用
            using var request = await FitAndShapeAPI.PostRequestJson(LOGIN_URL, GetUserAgent(), null,
                new LoginRequest(phoneNumber, password, "body-distortion", null));
            if (request.responseCode != 200)
            {
                throw new Exception(
                    $"Login Error. StatusCode:{request.responseCode}, Message:{request.error}");
            }

            LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            return loginResponse;
        }

        /// <summary>
        /// APIリクエストに必要なUserAgentの生成
        /// </summary>
        /// <returns></returns>
        public string GetUserAgent()
        {
            if (!string.IsNullOrWhiteSpace(_userAgent))
            {
                return _userAgent;
            }
#if UNITY_ANDROID
            const string systemStr = "android";
#else
            const string systemStr = "ios";
#endif
            _userAgent += $"_{FSConstants.WEBVIEW_URL_SCHEME}_{systemStr}_{Application.version}";

            return _userAgent;
        }

        public async UniTask<LoginData> GetLoginData()
        {
            if (_loginData == null)
            {
                _loginData = PlayerPrefsUtils.GetObject<LoginData>(LoginData.Key);
            }

            // 一定期間リフレッシュされていなかったら
            if (GetUnixTime() - _loginData.LastUpdatedAt > AUTH_REFRESH_RATE_SECONDS)
            {
                LoginInfo loginInfo = GetLoginInfo();
                if (!String.IsNullOrEmpty(loginInfo.UserId) && !String.IsNullOrEmpty(loginInfo.Password))
                {
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    CancellationToken cancellationToken = cancellationTokenSource.Token;
                    await AutoLogin(cancellationToken);
                }
            }

            return _loginData;
        }

        public LoginInfo GetLoginInfo()
        {
            return PlayerPrefsUtils.GetObject<LoginInfo>(LoginInfo.Key);
        }

        public void SetLoginData(LoginData loginData)
        {
            loginData.SetLastUpdatedAt(GetUnixTime());
            _loginData = loginData;
            PlayerPrefsUtils.SetObject(LoginData.Key, loginData);
        }
        
        public void SetLoginInfo(LoginInfo loginInfo)
        {
            PlayerPrefsUtils.SetObject(LoginInfo.Key, loginInfo);
        }

        private int GetUnixTime()
        {
            return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}