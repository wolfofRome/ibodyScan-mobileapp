using BestHTTP.WebSocket;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Amatib.ObjViewer.Infrastructure
{
    public class WebSocketClient
    {
        private readonly WebSocket _webSocket;
        private readonly string _apiHost;

        public WebSocketClient(string apiHost)
        {
            _apiHost = apiHost;
            _webSocket = new WebSocket(new Uri(_apiHost));
            _webSocket.Open();
        }

        public async UniTask DownloadAsync<T>(UnityAction<Dictionary<string, byte[]>> callback) where T : WebSocketData, new()
        {
            OnWebSocketMessageDelegate delegateOnMessage;
            OnWebSocketBinaryDelegate delegateOnBinary;

            T initializedData = new();

            int loadedPatternNum = 0;
            bool isComplete = false;
            Dictionary<string, byte[]> previewData = new Dictionary<string, byte[]>();

            _webSocket.OnMessage += delegateOnMessage = (WebSocket webSocket, string message) =>
            {
                // JsonUtilityはネストされたjsonを処理できない
                // initializedメッセージとバイナリ転送を別の通信で処理し結合
                initializedData = JsonUtility.FromJson<T>(message);
                isComplete = false;
                loadedPatternNum = 0;
                if (typeof(T) == typeof(TypedAdjustedData))
                {
                    previewData.Clear();
                }
            };

            _webSocket.OnBinary += delegateOnBinary = (WebSocket webSocket, byte[] data) =>
            {
                previewData.Add(initializedData.data[loadedPatternNum], data);

                loadedPatternNum++;

                if (loadedPatternNum == initializedData.data.Length)
                {
                    isComplete = true;
                    if (typeof(T) == typeof(TypedAdjustedData))
                    {
                        callback.Invoke(previewData);
                    }
                }
            };

            if (typeof(T) == typeof(TypedInitializedData))
            {
                TypedSelfData self = new TypedSelfData
                {
                    type = "self",
                    data = "webgl"
                };

                while (!_webSocket.IsOpen)
                {
                    await UniTask.Yield();
                }
                _webSocket.Send(JsonUtility.ToJson(self));

                while (!isComplete)
                {
                    await UniTask.Yield();
                }

                _webSocket.OnMessage -= delegateOnMessage;
                _webSocket.OnBinary -= delegateOnBinary;

                callback.Invoke(previewData);
            }
        }

        public void Close()
        {
            if (_webSocket.IsOpen)
            {
                _webSocket.Close();
            }
        }

        struct TypedSelfData
        {
            public string type;
            public string data;
        }

        [Serializable]
        public abstract class WebSocketData
        {
            public string type;
            public string[] data;
        }

        public class TypedInitializedData : WebSocketData
        {
        }

        public class TypedAdjustedData : WebSocketData
        {
        }
    }
}
