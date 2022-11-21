#if UNITY_ANDROID || UNITY_IOS
using System;
using System.Collections.Generic;
using System.Web;
using Gpm.WebView;
using UnityEngine;

namespace Iskra.Service.Platforms.Mobile
{
    public class MobileTermsService : ITermsService
    {
        private string _openWebUrl;
        private string _redirectUrl;
        private TermsService.TermsViewCallback _termsViewCallback;
        
        public void SetUrls(string openWebUrl, string redirectUrl)
        {
            _openWebUrl = openWebUrl;
            _redirectUrl = redirectUrl;
        }

        public void OpenWeb(string appId, string accessToken, TermsService.TermsViewCallback callback)
        {
            _termsViewCallback = callback;
            var url = string.Format("{0}?appId={1}&accessToken={2}", _openWebUrl, appId, accessToken);
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
            Debug.Log("OnMobileWebViewCallback: " + callbackType);
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
                            var agree = parameters["agree"];
                            var terms = new Terms
                            {
                                agree = (agree == "true")
                            };
                            _termsViewCallback.Invoke(terms, null);
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