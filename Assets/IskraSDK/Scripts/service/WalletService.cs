using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Web;
using AOT;
using Gpm.WebView;
using UnityEngine;

namespace Iskra
{
    public class WalletService
    {
        private string walletWebUrl;
        private string redirectUrl;

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
            this.walletWebUrl = walletWebUrl;
            this.redirectUrl = redirectUrl;
        }

        public void OpenWallet(string appId, string accessToken, string data, string userId)
        {
            var query = string.Format("?appId={0}&accessToken={1}&data={2}&userId={3}", appId, accessToken, data,
                userId);
            Open(walletWebUrl, query, Utils.GetBaseUrl(redirectUrl), Callback);
        }

        public void MobileOpenWallet(string appId, string accessToken, string data, string userId)
        {
            var url = string.Format("{0}?appId={1}&accessToken={2}&data={3}&userId={4}", 
                walletWebUrl,
                appId, 
                accessToken, 
                data,
                userId);
            Debug.Log("url:" + url);
            GpmWebView.ShowUrl(
                url,
                new GpmWebViewRequest.Configuration()
                {
                    style = GpmWebViewStyle.FULLSCREEN,
                    isClearCookie = true,
                    isClearCache = true,
                    isNavigationBarVisible = false,
                    navigationBarColor = "#4B96E6",
                    title = "The page title.",
                    isBackButtonVisible = true,
                    isForwardButtonVisible = true,
                    supportMultipleWindows = true,
#if UNITY_IOS
            contentMode = GpmWebViewContentMode.MOBILE
#endif
                },
                OnMobileWebViewCallback,
                new List<string>()
                {
                    "USER_ CUSTOM_SCHEME"
                });
        }


        private void OnMobileWebViewCallback(
            GpmWebViewCallback.CallbackType callbackType,
            string data,
            GpmWebViewError error)
        {
            switch (callbackType)
            {
                case GpmWebViewCallback.CallbackType.Open:
                    if (error != null)
                    {
                        Debug.LogFormat("Fail to open WebView. Error:{0}", error);
                    }

                    break;
                case GpmWebViewCallback.CallbackType.Close:
                    if (error != null)
                    {
                        Debug.LogFormat("Fail to close WebView. Error:{0}", error);
                    }

                    break;
                case GpmWebViewCallback.CallbackType.PageLoad:
                    if (string.IsNullOrEmpty(data) == false)
                    {
                        Debug.LogFormat("Loaded Page:{0}", data);
                        Uri uri = new Uri(data);
                        Uri redirectUri = new Uri(redirectUrl);
                        if (uri.AbsolutePath == redirectUri.AbsolutePath)
                        {
                            var parameters = HttpUtility.ParseQueryString(uri.Query);
                            var txHash = parameters["txHash"];
                            GpmWebView.Close();
                        }
                    }

                    break;
                case GpmWebViewCallback.CallbackType.MultiWindowOpen:
                    break;
                case GpmWebViewCallback.CallbackType.MultiWindowClose:
                    break;
                case GpmWebViewCallback.CallbackType.Scheme:
                    if (error == null)
                    {
                        if (data.Equals("USER_ CUSTOM_SCHEME") == true || data.Contains("CUSTOM_SCHEME") == true)
                        {
                            Debug.Log(string.Format("scheme:{0}", data));
                        }
                    }
                    else
                    {
                        Debug.Log(string.Format("Fail to custom scheme. Error:{0}", error));
                    }

                    break;
            }
        }
    }
}