#if UNITY_WEBGL
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace Iskra.Service.Platforms.WebGL
{
    public class WebGLWalletService : IWalletService
    {
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
            Debug.Log("WalletConnector#Callback: " + data);
            Close();
        }

        public void SetUrls(string walletWebUrl, string redirectUrl)
        {
            _walletWebUrl = walletWebUrl;
            _redirectUrl = redirectUrl;
        }

        public void OpenWallet(string appId, string accessToken, string data, string userId)
        {
            var query = string.Format("?appId={0}&accessToken={1}&data={2}&userId={3}", appId, accessToken, data,
                userId);
            Open(_walletWebUrl, query, Utils.GetBaseUrl(_redirectUrl), Callback);
            ;
        }
    }
}
#endif