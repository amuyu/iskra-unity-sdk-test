using System;
using System.Collections;
using Iskra.Api;
using UnityEngine;
using UnityEngine.Networking;

namespace Iskra.Service
{
    public class ConfigureManager
    {
        public const string CONFIG_LOCAL_API_HOST = "local.iskra.world:8080";
        public const string CONFIG_API_HOST = "api.iskra.cloud";
        public const string CONFIG_API_PATH = "/auth/v1/app/config";

        public Configuration Configuration;

        public delegate void GetConfigureCallback(Configuration configuration, Error error);


        public void GetConfigure(string phase, string appId, GetConfigureCallback callback)
        {
            BaseApi.Instance.Send(FetchConfigure(phase, appId, callback));
        }

        public IEnumerator FetchConfigure(string phase, string appId, GetConfigureCallback callback)
        {
            var url = GetConfigureUrl(phase);
            var downloadHandler = new DownloadHandlerBuffer();
            var uploadHandler = new UploadHandlerRaw(null);

            using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET, downloadHandler, uploadHandler))
            {
                // request.timeout = System.Convert.ToInt32(timeout);
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("X-Iskra-App-Id", appId);

                yield return request.SendWebRequest();

                var error = BaseApi.HandleHttpError(request);
                if (error != null)
                {
                    callback(null, error);
                }
                else
                {
                    var configureResponse = JsonUtility.FromJson<ConfigureResponse>(request.downloadHandler.text);

                    Configuration = new Configuration(
                        initialized: true,
                        phase: phase,
                        appId: appId,
                        authUrl: configureResponse.user.loginPageUrl,
                        authRedirectUrl: configureResponse.user.loginSuccessUrl,
                        verifyUrl: configureResponse.user.verifyUrl,
                        refreshTokenUrl: configureResponse.user.refreshTokenUrl,
                        termsUrl: configureResponse.user.termsPageUrl,
                        termsRedirectUrl: configureResponse.user.termsSuccessUrl,
                        termsVerifyUrl: configureResponse.user.termsVerifyUrl,
                        walletUrl: configureResponse.wallet.signaturePageUrl,
                        walletRedirectUrl: configureResponse.wallet.signatureSuccessUrl,
                        websocketUrl: configureResponse.wallet.connectUrl
                    );
                    callback(Configuration, null);
                }
            }
        }

        private string GetConfigureUrl(string phase)
        {
            if (string.IsNullOrEmpty(phase))
            {
                throw new ArgumentException("phase is not null or empty");
            }
            if (phase.ToLower() == "prod")
            {
                return string.Format("https://{0}{1}", CONFIG_API_HOST, CONFIG_API_PATH);
            }
            else if (phase.ToLower() == "local")
            {
                return string.Format("http://{0}{1}", CONFIG_LOCAL_API_HOST, CONFIG_API_PATH);
            }
            else
            {
                return string.Format("https://{0}-{1}{2}", phase, CONFIG_API_HOST, CONFIG_API_PATH);
            }
        }
    }
}