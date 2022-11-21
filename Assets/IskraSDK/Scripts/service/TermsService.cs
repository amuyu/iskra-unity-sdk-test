#if UNITY_ANDROID || UNITY_IOS
using Iskra.Service.Platforms.Mobile;
#elif UNITY_WEBGL
using Iskra.Service.Platforms.WebGL;
#endif
using Iskra.Service.Platforms;

namespace Iskra.Service
{
    public class TermsService : ITermsService
    {
        public ITermsService termsService;

        public delegate void TermsViewCallback(Terms terms, Error error);

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


        public void SetUrls(string openWebUrl, string redirectUrl)
        {
            termsService.SetUrls(openWebUrl, redirectUrl);
        }

        public void OpenWeb(string appId, string accessToken, TermsViewCallback callback)
        {
            termsService.OpenWeb(appId, accessToken, callback);
        }
    }
}