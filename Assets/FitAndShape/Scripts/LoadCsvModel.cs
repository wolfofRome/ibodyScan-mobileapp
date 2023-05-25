using BestHTTP;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace FitAndShape
{
    public interface ILoadCsvModel
    {
        UniTask<string> GetCsvData(string urlString, CancellationToken token);
        UniTask<(bool Result, string CsvData)> GetResult(string urlString, CancellationToken token);
    }

    public sealed class LoadCsvModel : ILoadCsvModel
    {
        async UniTask<string> ILoadCsvModel.GetCsvData(string urlString, CancellationToken token)
        {
            string result = string.Empty;

            var url = new Uri(urlString);

            var request = new HTTPRequest(url, HTTPMethods.Get);

            using (HTTPResponse response = await request.GetHTTPResponseAsync(token))
            {
                result = response.DataAsText;
            }

            request.Clear();

            return result;
        }

        async UniTask<(bool Result, string CsvData)> ILoadCsvModel.GetResult(string urlString, CancellationToken token)
        {
            string csvData = string.Empty;
            bool result = false;

            var url = new Uri(urlString);

            var request = new HTTPRequest(url, HTTPMethods.Get);

            using (HTTPResponse response = await request.GetHTTPResponseAsync(token))
            {
                csvData = response.DataAsText;

                result = !(response.StatusCode < 200 || response.StatusCode >= 400);
            }

            request.Clear();

            return (result, csvData);
        }
    }
}