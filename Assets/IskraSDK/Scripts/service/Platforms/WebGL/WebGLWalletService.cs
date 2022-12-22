#if UNITY_WEBGL
using System.Runtime.InteropServices;
using AOT;
using Iskra.Common;
using UnityEngine;

namespace Iskra.Service.Platforms.WebGL
{
    public class WebGLWalletService : IWalletService
    {
        private WalletService.OpenWalletCallback _openWalletCallback;
        private string _walletWebUrl;
        private string _redirectUrl;

        public delegate void WalletWebMessageCallback(string data);

        [DllImport("__Internal")]
        private static extern void Open(string url, string query, string redirectUrl, WalletWebMessageCallback action);

        [DllImport("__Internal")]
        private static extern void Close();

        [MonoPInvokeCallback(typeof(WalletWebMessageCallback))]
        public static void Callback(string data)
        {
            // Debug.Log("WalletConnector#Callback: " + data);
            Error error = null;
            if (data == null)
            {
                error = new Error
                {
                    code = ErrorCode.WEBVIEW_CLOSED.ToString(),
                    message = "WebView is closed."
                };
            }
            WalletService.Instance.walletService.GetOpenWalletCallback().Invoke(null, error);
            Close();
        }

        public void OpenWallet(string accessToken, string data, string userId, WalletService.OpenWalletCallback callback)
        {
            _openWalletCallback = callback;
            var configuration = IskraSDK.Instance.GetConfiguration();
            var query = string.Format("?appId={0}&accessToken={1}&data={2}&userId={3}", configuration.appId, accessToken, data,
                userId);
            Open(configuration.walletUrl, query, Utils.GetBaseUrl(configuration.walletRedirectUrl), Callback);
        }
        
        public WalletService.OpenWalletCallback GetOpenWalletCallback()
        {
            return _openWalletCallback;
        }
    }
}
#endif