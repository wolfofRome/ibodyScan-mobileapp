using System;
using System.IO;
using System.Web;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;

namespace Amatib.ObjViewer.Infrastructure
{
    /// <summary>
    /// APIクライアント
    /// </summary>
    public class ApiClient
    {
        private readonly string _apiHost;
        private readonly string _measurementNumber;
        private readonly string _key;
        private readonly string _token;

        public ApiClient(string apiHost, string measurementNumber, string key, string token)
        {
            _apiHost = apiHost;
            _measurementNumber = measurementNumber;
            _key = key;
            _token = token;

            //if(_measurementNumber == "FS2308086671")
            //    _token = "?token=9PNaHRfVMLZXDIrnVyOmzpZ24Y7KaFz9XgomZvhRXqk7E7s4XpyJENDtdYr1";
        }

        public async UniTask<MemoryStream> Download(string fileName)
        {
            //UriBuilder builder = new UriBuilder($"https://{_apiHost}/api/measurements/{_measurementNumber}/files/{fileName}");
            UriBuilder builder = new UriBuilder($"https://{_apiHost}/api/measurements/{_measurementNumber}/files/{fileName}");


            //Debug.Log($"_apiHost:{_apiHost}");

            //switch (_fitAndShapeServiceType)
            //{
            //    case FitAndShapeServiceType.Measuremenet:
            //        builder = new UriBuilder($"{MeasurementUrl}{_measurementNumber}/files/{fileName}");
            //        break;
            //    case FitAndShapeServiceType.Distortion:
            //        builder = new UriBuilder($"{DistortionUrl}{_measurementNumber}/files/{fileName}");
            //        break;
            //    default:
            //        builder = new UriBuilder($"https://{_apiHost}/api/measurements/{_measurementNumber}/files/{fileName}");
            //        break;
            //}

            //builder = new UriBuilder($"https://{_apiHost}/api/measurements/{_measurementNumber}/files/{fileName}");

            var query = HttpUtility.ParseQueryString(builder.Query);
            
            if (!string.IsNullOrEmpty(_key))
            {
                query["key"] = _key;
            }
            if (!string.IsNullOrEmpty(_token))
            {
                query["token"] = _token;
            }

            
            builder.Query = query.ToString();

            //Debug.Log(builder.ToString());

            using var request = UnityWebRequest.Get(builder.ToString());
            //using var request = UnityWebRequest.Get("https://api.fit-shape.jp/api/measurements/FS2308086671/files/scan_data.fbx?token=9PNaHRfVMLZXDIrnVyOmzpZ24Y7KaFz9XgomZvhRXqk7E7s4XpyJENDtdYr1");
            request.SetRequestHeader("Authorization", "9PNaHRfVMLZXDIrnVyOmzpZ24Y7KaFz9XgomZvhRXqk7E7s4XpyJENDtdYr1");
            Debug.LogError(builder.ToString());
            await request.SendWebRequest();
            return new MemoryStream(request.downloadHandler.data);
        }
    }
}