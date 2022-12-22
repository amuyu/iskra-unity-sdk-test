#if UNITY_WEBGL
using System;
using System.Runtime.InteropServices;
using AOT;
using Iskra.Common;
using UnityEngine;

namespace Iskra.Service.Platforms.WebGL
{
    public class WebGLTermsService : ITermsService
    {
        private TermsService.TermsViewCallback _termsViewCallback;

        public delegate void OnTermsWebOpenCallback(string data);

        [DllImport("__Internal")]
        private static extern void Open(string url, string query, string redirectUrl, OnTermsWebOpenCallback action);

        [DllImport("__Internal")]
        private static extern void Close();

        [MonoPInvokeCallback(typeof(OnTermsWebOpenCallback))]
        public static void Callback(string data)
        {
            // Debug.Log("TermsCallback.data:" + data);
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
                    var terms = JsonUtility.FromJson<Terms>(data);
                    if (terms != null)
                    {
                        TermsService.Instance.termsService.GetTermsViewCallback().Invoke(terms, null);
                        Close();
                        return;
                    }

                    error = new Error
                    {
                        code = ErrorCode.INTERNAL_ERROR.ToString(),
                        message = "Failed to agree to terms."
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


            TermsService.Instance.termsService.GetTermsViewCallback().Invoke(null, error);
            Close();
        }

        public void OpenWeb(string accessToken, TermsService.TermsViewCallback callback)
        {
            var configuration = IskraSDK.Instance.GetConfiguration();
            _termsViewCallback = callback;
            var query = string.Format("?appId={0}&accessToken={1}", configuration.appId, accessToken);
            Open(configuration.termsUrl, query, Utils.GetBaseUrl(configuration.termsRedirectUrl), Callback);
        }

        public TermsService.TermsViewCallback GetTermsViewCallback()
        {
            return _termsViewCallback;
        }
    }
}
#endif