#if UNITY_WEBGL
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace Iskra.Service.Platforms.WebGL
{
    public class WebGLTermsService : ITermsService
    {
        private string _openWebUrl;
        private string _redirectUrl;
        private TermsService.TermsViewCallback _termsViewCallback;

        public delegate void OnTermsWebOpenCallback(string data);

        [DllImport("__Internal")]
        private static extern void Open(string url, string query, string redirectUrl, OnTermsWebOpenCallback action);

        [DllImport("__Internal")]
        private static extern void Close();

        [MonoPInvokeCallback(typeof(OnTermsWebOpenCallback))]
        public static void Callback(string data)
        {
            var terms = JsonUtility.FromJson<Terms>(data);
            if (TermsService.Instance.termsService is WebGLTermsService termsService)
            {
                termsService._termsViewCallback.Invoke(terms, null);
            }

            Close();
        }


        public void SetUrls(string openWebUrl, string redirectUrl)
        {
            _openWebUrl = openWebUrl;
            _redirectUrl = redirectUrl;
        }

        public void OpenWeb(string appId, string accessToken, TermsService.TermsViewCallback callback)
        {
            _termsViewCallback = callback;
            var query = string.Format("?appId={0}&accessToken={1}", appId, accessToken);
            Open(_openWebUrl, query, Utils.GetBaseUrl(_redirectUrl), Callback);
        }
    }
}
#endif