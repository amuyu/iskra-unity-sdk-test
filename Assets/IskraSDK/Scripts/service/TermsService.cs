#if UNITY_ANDROID || UNITY_IOS
using Iskra.Service.Platforms.Mobile;
#elif UNITY_WEBGL
using Iskra.Service.Platforms.WebGL;
#endif
using System.Collections;
using Iskra.Api;
using Iskra.Service.Platforms;
using UnityEngine;
using UnityEngine.Networking;

namespace Iskra.Service
{
    public class TermsService
    {
        public ITermsService termsService;

        public delegate void TermsViewCallback(Terms terms, Error error);

        public delegate void GetTermsAgreeCallback(Terms terms, Error error);

        public TermsService()
        {
#if UNITY_ANDROID || UNITY_IOS
            termsService = new MobileTermsService();
#elif UNITY_WEBGL
            termsService = new WebGLTermsService();
#endif
        }

        private static TermsService instance;

        public static TermsService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TermsService();
                }

                return instance;
            }
        }

        public void OpenWeb(string accessToken, TermsViewCallback callback)
        {
            termsService.OpenWeb(accessToken, callback);
        }

        public void GetTermsAgree(string accessToken, GetTermsAgreeCallback callback)
        {
            BaseApi.Instance.Send(FetchTermsVerify(accessToken, callback));
        }

        public IEnumerator FetchTermsVerify(string accessToken, GetTermsAgreeCallback callback)
        {
            var configuration = IskraSDK.Instance.GetConfiguration();
            var downloadHandler = new DownloadHandlerBuffer();
            var uploadHandler = new UploadHandlerRaw(null);

            using (var request = new UnityWebRequest(configuration.termsVerifyUrl + "?country=US",
                       UnityWebRequest.kHttpVerbGET, downloadHandler,
                       uploadHandler))
            {
                // request.timeout = System.Convert.ToInt32(timeout);
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("X-Iskra-App-Id", configuration.appId);
                request.SetRequestHeader("Authorization", "Bearer " + accessToken);

                yield return request.SendWebRequest();

                var error = BaseApi.HandleHttpError(request);
                if (error != null)
                {
                    callback(null, error);
                }
                else
                {
                    var termsAgreeResponse = JsonUtility.FromJson<TermsAgreeResponse>(request.downloadHandler.text);
                    callback(termsAgreeResponse.toTerms(), null);
                }
            }
        }
    }
}