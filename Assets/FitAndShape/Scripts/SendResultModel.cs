using BestHTTP;
using Cysharp.Threading.Tasks;
using System;
using System.Text;
using System.Threading;
using UnityEngine;

namespace FitAndShape
{
    public interface ISendResultModel
    {
        UniTask Put(string uriString, string json, CancellationToken token);
    }

    public sealed class SendResultModel : ISendResultModel
    {
        async UniTask ISendResultModel.Put(string uriString, string json, CancellationToken token)
        {
            Uri uri = new Uri(uriString);

            var request = new HTTPRequest(uri, HTTPMethods.Put);
            request.SetHeader("Content-Type", "application/json");

            string data = @"{""data"":" + json + "}";

            //Debug.Log(uriString);
            //Debug.Log(data);

            request.RawData = Encoding.UTF8.GetBytes(data);

            await request.Send();

            //Debug.Log(request.Response.StatusCode);

            request.Clear();
        }
    }
}