#if UNITY_WEBGL
using System;
using System.Runtime.InteropServices;
using AOT;
using Iskra.Common;
using UnityEngine;

namespace Iskra.Service.Platforms.WebGL
{
    public class WebGLAuthService : IAuthService
    {
        AuthService.LoginCallback _loginCallback;
        private IAuthTokenManager authTokenManager;

        public delegate void OnOpenCallback(string data);

        [DllImport("__Internal")]
        private static extern void Open(string url, string query, string redirectUrl, OnOpenCallback action);

        [DllImport("__Internal")]
        private static extern void Close();

        [MonoPInvokeCallback(typeof(OnOpenCallback))]
        public static void Callback(string data)
        {
            // Debug.Log("LoginCallback.data:" + data);
            Error error = null;
            if (data == null)
            {
                error = new Error
                {
                    code = ErrorCode.WEBVIEW_CLOSED.ToString(),
                    message = "WebView is closed."
                };
            }
            else
            {
                try
                {
                    var auth = JsonUtility.FromJson<Auth>(data);
                    if (auth != null && !auth.IsEmpty())
                    {
                        IskraSDK.Instance.auth = auth;
                        AuthService.Instance.authService.StoreToken(auth);
                        AuthService.Instance.authService.GetLoginCallback().Invoke(IskraSDK.Instance.auth, null);
                        Close();
                        return;
                    }

                    error = new Error
                    {
                        code = ErrorCode.INTERNAL_ERROR.ToString(),
                        message = "Failed to login."
                    };
                }
                catch (Exception e)
                {
                    error = new Error
                    {
                        code = ErrorCode.INTERNAL_ERROR.ToString(),
                        message = e.Message
                    };
                }
            }

            AuthService.Instance.authService.GetLoginCallback().Invoke(null, error);
            Close();
        }

        public void StoreToken(Auth auth)
        {
            var authToken = JsonUtility.ToJson(auth);
            GetAuthTokenManager().StoreToken(authToken);
        }

        public Auth GetLastLogin()
        {
            var authToken = GetAuthTokenManager().GetToken();
            if (authToken == null || string.IsNullOrEmpty(authToken)) return null;
            var auth = JsonUtility.FromJson<Auth>(authToken);
            IskraSDK.Instance.auth = auth;
            return auth;
        }

        public void Login(AuthService.LoginCallback callback)
        {
            var configuration = IskraSDK.Instance.GetConfiguration();
            _loginCallback = callback;
            var query = "?appId=" + configuration.appId;
            Open(configuration.authUrl, query, Utils.GetBaseUrl(configuration.authRedirectUrl), Callback);
        }

        public void Logout(AuthService.LogoutCallback callback)
        {
            GetAuthTokenManager().RemoveToken();
            callback(null);
        }

        private IAuthTokenManager GetAuthTokenManager()
        {
            if (authTokenManager == null)
            {
                var configuration = IskraSDK.Instance.GetConfiguration();
                authTokenManager = new WebGLAuthTokenManager(configuration.appId);
            }

            return authTokenManager;
        }

        public AuthService.LoginCallback GetLoginCallback()
        {
            return _loginCallback;
        }
    }
}
#endif