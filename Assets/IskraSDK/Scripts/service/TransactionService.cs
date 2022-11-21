using System;
using NativeWebSocket;
using UnityEngine;

namespace Iskra.Service
{
    public class TransactionService : MonoBehaviour, IWebSocketAdapter
    {
        private string webSocketUrl;
        private ProgressEventCallback.ProgressDelegate _callback;
        private WalletOpenDelegate _walletOpencallback;

        public delegate void WalletOpenDelegate(string data);

        public void SetWebSocketUrl(string url)
        {
            webSocketUrl = url;
        }

        public void PrepareSigning(
            ProgressEventCallback.ProgressDelegate callback,
            WalletOpenDelegate walletOpenCallback)
        {
            _callback = callback;
            _walletOpencallback = walletOpenCallback;
            WebSocketManager.Instance.Connect(webSocketUrl, this);
        }

        public void WebSocketClose()
        {
            WebSocketManager.Instance.Close();
        }

        public void OnOpen()
        {
            _callback(ProgressEventCallback.Type.OnStart, Result.EmptyData(true));
        }

        public void OnError(string e)
        {
        }

        public void OnClose(WebSocketCloseCode code)
        {
            _callback(ProgressEventCallback.Type.OnClose, Result.EmptyData(true)); 
        }

        public void OnMessage(string messageStr)
        {
            var message = JsonUtility.FromJson<WebSocketMessage>(messageStr);
            try
            {
                WebSocketMessage.Type type =
                    (WebSocketMessage.Type)Enum.Parse(typeof(WebSocketMessage.Type), message.type);
                switch (type)
                {
                    case WebSocketMessage.Type.signature:
                        _walletOpencallback(Utils.Base64Encode(messageStr));
                        break;
                    case WebSocketMessage.Type.txReceipt:
                        var transactionResult = JsonUtility.FromJson<TransactionResult>(messageStr);
                        var status = transactionResult.doGetReceipt == "SUCCESS";
                        _callback(ProgressEventCallback.Type.OnFinish, Result.EmptyData(status));
                        WebSocketManager.Instance.Close();
                        break;
                }
            }
            catch (ArgumentException)
            {
                // Unknown message type
            }
        }
    }

    public class WebSocketMessage
    {
        public string type;


        public enum Type
        {
            signature,
            txReceipt
        }
    }
}