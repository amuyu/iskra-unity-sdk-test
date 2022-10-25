using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Web;
using AOT;
using Gpm.WebView;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Iskra
{
    public class AuthService
    {
        private string openWebUrl;
        private string redirectUrl;

        public delegate void LoginCallback(Auth data, Error error);

        public delegate void LogoutCallback(Error error);

# if UNITY_WEBGL
        public delegate void OnOpenCallback(string data);

        [DllImport("__Internal")]
        private static extern void Open(string url, string query, string redirectUrl, OnOpenCallback action);

        [DllImport("__Internal")]
        private static extern void Close();

        [MonoPInvokeCallback(typeof(OnOpenCallback))]
        public static void Callback(string data)
        {
            var auth = JsonUtility.FromJson<Auth>(data);
            if (auth != null && !string.IsNullOrEmpty(auth.accessToken))
            {
                IskraSDK.Instance.auth = auth;
            }
            _loginCallback.Invoke(IskraSDK.Instance.auth, null);
            Close();
        }
#endif

        private static LoginCallback _loginCallback;

        public void SetUrls(string openWebUrl, string redirectUrl)
        {
            this.openWebUrl = openWebUrl;
            this.redirectUrl = redirectUrl;
        }

# if UNITY_WEBGL
        // WebGL
        public void SignIn(string appId, LoginCallback callback)
        {
            _loginCallback = callback;
            var query = "?appId=" + appId;
            Open(openWebUrl, query, Utils.GetBaseUrl(redirectUrl), Callback);
        }
#endif

        public void MobileSignIn(string appId, LoginCallback callback)
        {
            _loginCallback = callback;
            var url = string.Format("{0}?appId={1}", openWebUrl, appId);
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
                            var auth = new Auth
                            {
                                userId = parameters["userId"],
                                accessToken = parameters["accessToken"],
                                refreshToken = parameters["refreshToken"],
                                walletAddress = parameters["walletAddress"]
                            };
                            if (auth != null && !string.IsNullOrEmpty(auth.accessToken))
                            {
                                IskraSDK.Instance.auth = auth;
                            }
                            _loginCallback.Invoke(IskraSDK.Instance.auth, null);
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