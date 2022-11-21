#if UNITY_WEBGL
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace Iskra.Service.Platforms.WebGL
{
    public class WebGLAuthService : IAuthService
    {
        private string _openWebUrl;
        private string _redirectUrl;
        AuthService.LoginCallback _loginCallback;
        
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

            if (AuthService.Instance.authService is WebGLAuthService authService)
            {
                authService._loginCallback.Invoke(IskraSDK.Instance.auth, null);
            }
            Close();
        }
        
        
        public void SetUrls(string openWebUrl, string redirectUrl)
        {
            _openWebUrl = openWebUrl;
            _redirectUrl = redirectUrl;
        }

        public void SignIn(string appId, AuthService.LoginCallback callback)
        {
            _loginCallback = callback;
            var query = "?appId=" + appId;
            Open(_openWebUrl, query, Utils.GetBaseUrl(_redirectUrl), Callback);
        }
    }
}
#endif