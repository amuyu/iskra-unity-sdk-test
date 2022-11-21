using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Iskra.Service
{
    public class ConfigureManager : MonoBehaviour
    {
        public const string ConfigureApiHost = "api.iskra.cloud";
        public const string ConfigureApiPath = "/auth/v1/app/config";
        
        private static ConfigureManager instance = null;

        public static ConfigureManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject("IskraConfigureManager");
                    instance = obj.AddComponent<ConfigureManager>();
                    DontDestroyOnLoad(obj);
                }

                return instance;
            }
        }

        public delegate void GetConfigureCallback(Configuration configuration, Error error);

        public delegate void GetTermsAgreeCallback(Terms terms, Error error);

        public void GetConfigure(string phase, string appId, GetConfigureCallback callback)
        {
            var url = GetConfigureUrl(phase);
            StartCoroutine(FetchConfigure(appId, url, callback));
        }

        public void GetTermsAgree(string appId, string accessToken, string url, GetTermsAgreeCallback callback)
        {
            StartCoroutine(FetchTermsVerify(appId, accessToken, url, callback));
        }

        public IEnumerator FetchConfigure(string appId, string url, GetConfigureCallback callback)
        {
            var downloadHandler = new DownloadHandlerBuffer();
            var uploadHandler = new UploadHandlerRaw(null);

            using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET, downloadHandler, uploadHandler))
            {
                // request.timeout = System.Convert.ToInt32(timeout);
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("X-Iskra-App-Id", appId);

                yield return request.SendWebRequest();

                var configureResponse = JsonUtility.FromJson<ConfigureResponse>(request.downloadHandler.text);
                var configuration = new Configuration
                {
                    appId = appId,
                    authUrl = configureResponse.user.loginPageUrl,
                    authRedirectUrl = configureResponse.user.loginSuccessUrl,
                    verifyUrl = configureResponse.user.verifyUrl,
                    termsUrl = configureResponse.user.termsPageUrl,
                    termsRedirectUrl = configureResponse.user.termsSuccessUrl,
                    termsVerifyUrl = configureResponse.user.termsVerifyUrl,
                    walletUrl = configureResponse.wallet.signaturePageUrl,
                    walletRedirectUrl = configureResponse.wallet.signatureSuccessUrl,
                    websocketUrl = configureResponse.wallet.connectUrl
                };
                callback(configuration, null);
            }
        }

        public IEnumerator FetchTermsVerify(string appId, string accessToken, string url,
            GetTermsAgreeCallback callback)
        {
            var downloadHandler = new DownloadHandlerBuffer();
            var uploadHandler = new UploadHandlerRaw(null);

            using (var request = new UnityWebRequest(url + "?country=US", UnityWebRequest.kHttpVerbGET, downloadHandler, uploadHandler))
            {
                // request.timeout = System.Convert.ToInt32(timeout);
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("X-Iskra-App-Id", appId);
                request.SetRequestHeader("Authorization", "Bearer " + accessToken);

                yield return request.SendWebRequest();

                var termsAgreeResponse = JsonUtility.FromJson<TermsAgreeResponse>(request.downloadHandler.text);
                callback(termsAgreeResponse.toTerms(), null);
            }
        }

        private string GetConfigureUrl(string phase)
        {
            if (phase.ToLower() == "prod")
            {
                return string.Format("https://{0}{1}", ConfigureApiHost, ConfigureApiPath);
            }
            else
            {
                return string.Format("https://{0}-{1}{2}", phase, ConfigureApiHost, ConfigureApiPath);
            }
        }
    }
}