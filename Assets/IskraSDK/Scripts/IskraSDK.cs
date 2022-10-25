using System;

namespace Iskra
{
    public class IskraSDK
    {
        public const string VERSION = "0.1.0";

        private AuthService _authService;
        private TermsService _termsService;
        private WalletService _walletService;
        private TransactionService _transactionService;

        private Configuration _configuration;
        public Auth auth;
        private bool initialized = false;

        public delegate void IntitializeCallback(Error error);

        public IskraSDK()
        {
            _authService = new AuthService();
            _termsService = new TermsService();
            _walletService = new WalletService();
            _transactionService = new TransactionService();
        }

        private static IskraSDK instance;

        public static IskraSDK Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new IskraSDK();
                }

                return instance;
            }
        }

        public void Initialize(string phase, string appId, IntitializeCallback callback)
        {
            ConfigureManager.Instance.GetConfigure(phase, appId, (configuration, error) =>
            {
                initialized = (error == null);
                this._configuration = configuration;
                _authService.SetUrls(this._configuration.authUrl, this._configuration.authRedirectUrl);
                _walletService.SetUrls(this._configuration.walletUrl, this._configuration.walletRedirectUrl);
                _termsService.SetUrls(this._configuration.termsUrl, this._configuration.termsRedirectUrl);
                callback(error);
            });
        }

        public void Login(AuthService.LoginCallback callback)
        {
#if UNITY_ANDROID || UNITY_IOS
            _authService.MobileSignIn(_configuration.appId, callback);
#else
            _authService.SignIn(_configuration.appId, callback);
#endif
        }

        public void GetTermsAgree(ConfigureManager.GetTermsAgreeCallback callback)
        {
            ConfigureManager.Instance.GetTermsAgree(
                _configuration.appId,
                auth.accessToken,
                _configuration.termsVerifyUrl,
                callback);
        }

        public void ShowTermsView(TermsService.TermsViewCallback callback)
        {
#if UNITY_ANDROID || UNITY_IOS
            _termsService.MobileOpenTermsWeb(_configuration.appId, auth.accessToken, callback);
#else
            _termsService.OpenWeb(_configuration.appId, auth.accessToken, callback);
#endif
        }

        public void Logout(AuthService.LogoutCallback callback)
        {
            callback(null);
            auth = null;
        }

        public void PrepareSigning(ProgressEventCallback.ProgressDelegate callback)
        {
            var websocketUrl = string.Format("{0}?appId={1}&accessToken={2}", _configuration.websocketUrl,
                _configuration.appId, auth.accessToken);
            _transactionService.SetWebSocketUrl(websocketUrl);
            _transactionService.PrepareSigning(callback, data =>
            {
#if UNITY_ANDROID || UNITY_IOS
                _walletService.MobileOpenWallet(_configuration.appId, auth.accessToken, data, auth.userId);
#else
                _walletService.OpenWallet(_configuration.appId, auth.accessToken, data, auth.userId);
#endif
            });
        }
    }

    public static class Utils
    {
        public static string GetBaseUrl(string url)
        {
            Uri uri = new Uri(url);
            return uri.AbsoluteUri.Replace(uri.PathAndQuery, "");
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}