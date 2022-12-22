using NativeWebSocket;
using UnityEngine;
using Logger = Iskra.Common.Logger;

namespace Iskra.Service
{
    public interface IWebSocketAdapter
    {
        void OnOpen();
        void OnError(string e);
        void OnClose(WebSocketCloseCode code);
        void OnMessage(string message);
    }
    
    public class WebSocketManager : MonoBehaviour
    {
        private WebSocket _webSocket;
        private static WebSocketManager instance = null;

        public static WebSocketManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject("IskraWebSocketManager");
                    instance = obj.AddComponent<WebSocketManager>();
                    DontDestroyOnLoad(obj);
                }

                return instance;
            }
        }

        public async void Connect(string url, IWebSocketAdapter webSocketAdapter)
        {
            _webSocket = new WebSocket(url);
            _webSocket.OnOpen += () =>
            {
                Logger.Debug("Connection open!", this);
                webSocketAdapter.OnOpen();
                SendWebSocketMessage("{\"action\":\"ping\",\"type\":\"client\"}");
            };

            _webSocket.OnError += (e) =>
            {
                Logger.Debug("Error! " + e, this);
                webSocketAdapter.OnError(e);
            };

            _webSocket.OnClose += (e) =>
            {
                Logger.Debug("Connection closed! code:" + e, this);
                webSocketAdapter.OnClose(e);
                _webSocket = null;
            };

            _webSocket.OnMessage += (bytes) =>
            {
                // Reading a plain text message
                var messageStr = System.Text.Encoding.UTF8.GetString(bytes);
                Logger.Debug("Received OnMessage! (" + bytes.Length + " bytes) " + messageStr, this);
                webSocketAdapter.OnMessage(messageStr);
            };
            await _webSocket.Connect();
        }

        public async void Close()
        {
            if (_webSocket != null)
            {
                await _webSocket.Close();
                _webSocket = null;
            } 
        }

        void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (_webSocket != null)
            {
                _webSocket.DispatchMessageQueue();
            }
#endif
        }

        async void SendWebSocketMessage(string message)
        {
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                // Sending bytes
                // await websocket.Send(new byte[] { 10, 20, 30 });

                // Sending plain text
                // await websocket.SendText("plain text message");
                await _webSocket.SendText(message);
            }
        }
        
        private async void OnApplicationQuit()
        {
            Close();
        }
    }
}