#if UNITY_ANDROID || UNITY_IOS
using Iskra.Service.Platforms.Mobile;
#elif UNITY_WEBGL
using Iskra.Service.Platforms.WebGL;
#endif
using Iskra.Service.Platforms;

namespace Iskra.Service
{
    public class AuthService : IAuthService
    {
        public IAuthService authService;

        public delegate void LoginCallback(Auth data, Error error);

        public delegate void LogoutCallback(Error error);

        public AuthService()
        {
#if UNITY_ANDROID || UNITY_IOS
            authService = new MobileAuthService();
#elif UNITY_WEBGL
            authService = new WebGLAuthService();
#endif
        }

        private static AuthService instance;

        public static AuthService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AuthService();
                }

                return instance;
            }
        }


        public void SetUrls(string openWebUrl, string redirectUrl)
        {
            authService.SetUrls(openWebUrl, redirectUrl);
        }

        // WebGL
        public void SignIn(string appId, LoginCallback callback)
        {
            authService.SignIn(appId, callback);
        }
    }
}