#if UNITY_ANDROID || UNITY_IOS
using System;
using System.Collections.Generic;
using System.Web;
using Gpm.WebView;
using Iskra.Common;
using UnityEngine;
using Logger = Iskra.Common.Logger;

namespace Iskra.Service.Platforms.Mobile
{
    public class MobileAuthService : IAuthService
    {
        private AuthService.LoginCallback _loginCallback;
        private IAuthTokenManager authTokenManager;

        public MobileAuthService()
        {
#if UNITY_ANDROID
            authTokenManager = new AndroidAuthTokenManager();
#elif UNITY_IOS
            authTokenManager = new IOSAuthTokenManager();
#endif
            authTokenManager.Initialize();
        }

        public void StoreToken(Auth auth)
        {
            var authToken = JsonUtility.ToJson(auth);
            authTokenManager.StoreToken(authToken);
        }

        public void Logout(AuthService.LogoutCallback callback)
        {
            authTokenManager.RemoveToken();
            callback(null);
        }

        public Auth GetLastLogin()
        {
            var authToken = authTokenManager.GetToken();
            if (authToken == null || string.IsNullOrEmpty(authToken)) return null;
            var auth = JsonUtility.FromJson<Auth>(authToken);
            IskraSDK.Instance.auth = auth;
            return auth;
        }

        public void Login(AuthService.LoginCallback callback)
        {
            var configuration = IskraSDK.Instance.GetConfiguration();
            _loginCallback = callback;
            var url = string.Format("{0}?appId={1}", configuration.authUrl, configuration.appId);
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
                        Uri redirectUri = new Uri(configuration.authRedirectUrl);
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
                            var authToken = JsonUtility.ToJson(auth);
                            authTokenManager.StoreToken(authToken);
                            if (auth != null && !string.IsNullOrEmpty(auth.accessToken))
                            {
                                IskraSDK.Instance.auth = auth;
                            }

                            // _loginCallback.Invoke(IskraSDK.Instance.auth, null);
                            InvokeCallback(IskraSDK.Instance.auth, null);
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
                case GpmWebViewCallback.CallbackType.GoBack:
                    Logger.Debug("GoBack!!", this);
                    break;
            }
        }

        public AuthService.LoginCallback GetLoginCallback()
        {
            return _loginCallback;
        }

        void InvokeCallback(Auth data, Error error)
        {
            if (_loginCallback != null)
            {
                _loginCallback.Invoke(data, error);
                _loginCallback = null;
            }
        }
    }
}
#endif