#if UNITY_ANDROID || UNITY_IOS
using System;
using System.Collections.Generic;
using System.Web;
using Gpm.WebView;
using Iskra.Common;

namespace Iskra.Service.Platforms.Mobile
{
    public class MobileWalletService : IWalletService
    {
        private WalletService.OpenWalletCallback _openWalletCallback;

        public void OpenWallet(string accessToken, string data, string userId, WalletService.OpenWalletCallback callback)
        {
            _openWalletCallback = callback;
            var configuration = IskraSDK.Instance.GetConfiguration();
            var url = string.Format("{0}?appId={1}&accessToken={2}&data={3}&userId={4}",
                configuration.walletUrl,
                configuration.appId,
                accessToken,
                data,
                userId);
            GpmWebView.ShowUrl(
                url,
                new GpmWebViewRequest.Configuration()
                {
                    style = GpmWebViewStyle.FULLSCREEN,
                    isClearCookie = true,
                    isClearCache = true,
                    isNavigationBarVisible = true,
                    navigationBarColor = "#232441",
                    title = " ",
                    isBackButtonVisible = false,
                    isForwardButtonVisible = false,
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
                        Logger.Debug(string.Format("Fail to open WebView. Error:{0}", error), this);
                    }

                    break;
                case GpmWebViewCallback.CallbackType.Close:
                    if (error != null)
                    {
                        Logger.Debug(string.Format("Fail to close WebView. Error:{0}", error), this);
                    }
                    
                    InvokeCallback(null, new Error
                    {
                        code = ErrorCode.WEBVIEW_CLOSED.ToString(),
                        message = "WebView is closed."
                    });
                    break;
                case GpmWebViewCallback.CallbackType.PageLoad:
                    if (string.IsNullOrEmpty(data) == false)
                    {
                        // Debug.LogFormat("Loaded Page:{0}", data);
                        Uri uri = new Uri(data);
                        var configuration = IskraSDK.Instance.GetConfiguration();
                        Uri redirectUri = new Uri(configuration.walletRedirectUrl);
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
                            Logger.Debug(string.Format("scheme:{0}", data), this);
                        }
                    }
                    else
                    {
                        Logger.Debug(string.Format("Fail to custom scheme. Error:{0}", error), this);
                    }

                    break;
            }
        }

        public WalletService.OpenWalletCallback GetOpenWalletCallback()
        {
            return _openWalletCallback;
        }
        
        void InvokeCallback(string data, Error error)
        {
            if (_openWalletCallback != null)
            {
                _openWalletCallback.Invoke(data, error);
                _openWalletCallback = null;
            }
        }
    }
}
#endif