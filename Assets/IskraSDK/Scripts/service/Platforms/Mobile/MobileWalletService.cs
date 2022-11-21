#if UNITY_ANDROID || UNITY_IOS
using System;
using System.Collections.Generic;
using System.Web;
using Gpm.WebView;
using UnityEngine;

namespace Iskra.Service.Platforms.Mobile
{
    public class MobileWalletService : IWalletService
    {
        private string _walletWebUrl;
        private string _redirectUrl;


        public void SetUrls(string walletWebUrl, string redirectUrl)
        {
            _walletWebUrl = walletWebUrl;
            _redirectUrl = redirectUrl;
        }

        public void OpenWallet(string appId, string accessToken, string data, string userId)
        {
            var url = string.Format("{0}?appId={1}&accessToken={2}&data={3}&userId={4}", 
                _walletWebUrl,
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
                        Uri redirectUri = new Uri(_redirectUrl);
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
#endif