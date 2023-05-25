using System;
using System.IO;
using System.Web;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

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
        }

        public async UniTask<MemoryStream> Download(string fileName)
        {
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
            await request.SendWebRequest();
            return new MemoryStream(request.downloadHandler.data);
        }
    }
}