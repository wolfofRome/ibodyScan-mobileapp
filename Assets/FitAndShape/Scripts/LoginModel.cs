using System;
using System.Threading;
using BestHTTP;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace FitAndShape
{
    public interface ILoginModel
    {
        bool IsLogin { get; }

        UniTask<LoginResponse> Login(string phoneNumber, string password, CancellationToken token);

    }

    public class LoginModel : ILoginModel
    {
        readonly static string URL = "https://api.fit-shape.jp/api/app/login";
        readonly static string DEMO_URL = "https://api.fit-shape.jp/api/app/demo-login";

        public bool IsLogin { get; private set; } = false;

        readonly string UserAgent;

        public LoginModel(string userAgent)
        {
            UserAgent = userAgent;
        }

        async UniTask<LoginResponse> ILoginModel.Login(string phoneNumber, string password, CancellationToken token)

        {
            IsLogin = false;

            HTTPRequest request = null;


            if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(password))

            {
                Uri uri = new Uri(DEMO_URL);

                request = new HTTPRequest(uri, HTTPMethods.Post);
                request.SetHeader("Content-Type", "application/json");
                request.SetHeader("user-agent", UserAgent);

                request.AddField("redirect", "body-distortion");
                request.AddField("is_demo", "true");
            }
            else
            {
                Uri uri = new Uri(URL);

                request = new HTTPRequest(uri, HTTPMethods.Post);
                request.SetHeader("Content-Type", "application/json");
                request.SetHeader("user-agent", UserAgent);

                request.AddField("tel", phoneNumber);
                request.AddField("password", password);
                request.AddField("redirect", "body-distortion");
            }

            await request.Send();

            request.Clear();

            if (request.Response.StatusCode != 200)
            {
                throw new Exception($"Login Error. StatusCode:{request.Response.StatusCode}, Message:{request.Response.Message}");
            }

            string dataAsText = request.Response.DataAsText;

            IsLogin = true;

            return JsonUtility.FromJson<LoginResponse>(dataAsText);
        }
    }
}