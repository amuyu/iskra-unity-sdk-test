using System;
using System.Text;
using Iskra.Common;
using Iskra.Service;

namespace Iskra
{
    public class IskraSDK
    {
        public const string VERSION = "0.9.2";

        private TransactionService _transactionService;
        private ConfigureManager _configureManager;
        public Auth auth;
        public Wallet wallet;
        private bool initialized = false;

        public delegate void IntitializeCallback(Error error);

        public IskraSDK()
        {
            _transactionService = new TransactionService();
            _configureManager = new ConfigureManager();
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
            _configureManager.GetConfigure(phase, appId, (configuration, error) =>
            {
                initialized = true;
                callback(error);
            });
        }

        public void LoadLastLogin(AuthService.LoginCallback callback)
        {
            if (CheckInitializedWithDataErrorCallback(new DataErrorCallback<Auth>(callback)))
            {
                AuthService.Instance.LoadLastLogin(callback);
            }
        }

        public void Login(AuthService.LoginCallback callback)
        {
            if (CheckInitializedWithDataErrorCallback(new DataErrorCallback<Auth>(callback)))
            {
                AuthService.Instance.Login(callback);
            }
        }

        public void VerifyAccessToken(AuthService.VerifyAccessTokenCallback callback)
        {
            if (CheckInitializedWithErrorCallback(new ErrorCallback(callback)))
            {
                AuthService.Instance.VerifyAccessToken(auth.accessToken, callback);
            }
        }

        public void RefreshAccessToken(AuthService.LoginCallback callback)
        {
            if (CheckInitializedWithDataErrorCallback(new DataErrorCallback<Auth>(callback)))
            {
                AuthService.Instance.RefreshAccessToken(
                    auth.accessToken,
                    auth.refreshToken,
                    callback);
            }
        }

        public void GetWallet(AuthService.GetWalletCallback callback)
        {
            if (CheckInitializedWithDataErrorCallback(new DataErrorCallback<Wallet>(callback)))
            {
                AuthService.Instance.GetWallet(
                    auth.accessToken,
                    callback);
            }
        }

        public void GetTermsAgree(TermsService.GetTermsAgreeCallback callback)
        {
            if (CheckInitializedWithDataErrorCallback(new DataErrorCallback<Terms>(callback)))
            {
                TermsService.Instance.GetTermsAgree(auth.accessToken, callback);
            }
        }

        public void ShowTermsView(TermsService.TermsViewCallback callback)
        {
            if (CheckInitializedWithDataErrorCallback(new DataErrorCallback<Terms>(callback)))
            {
                TermsService.Instance.OpenWeb(auth.accessToken, callback);
            }
        }

        public void Logout(AuthService.LogoutCallback callback)
        {
            if (CheckInitializedWithErrorCallback(new ErrorCallback(callback)))
            {
                auth = null;
                AuthService.Instance.Logout(callback);
            }
        }

        public void PrepareSigning(ProgressEventCallback.ProgressDelegate callback)
        {
            if (CheckInitializedWithProgressDelegate(callback))
            {
                var configuration = GetConfiguration();
                var websocketUrl = string.Format("{0}?appId={1}&accessToken={2}", configuration.websocketUrl,
                    configuration.appId, auth.accessToken);
                _transactionService.SetWebSocketUrl(websocketUrl);
                _transactionService.PrepareSigning(callback,
                    data =>
                    {
                        WalletService.Instance.OpenWallet(auth.accessToken, data, auth.userId, (s, error) =>
                        {
                            if (error != null)
                            {
                                callback(ProgressEventCallback.Type.OnFinish, Result.Error(error));
                            }
                        });
                    });
            }
        }

        public string GetAccessToken()
        {
            var auth = this.auth ?? AuthService.Instance.GetLastLogin();
            if (auth == null || auth.IsEmpty())
            {
                return null;
            }

            return auth.accessToken;
        }

        public string GetVersion()
        {
            return VERSION;
        }

        private bool CheckInitializedWithErrorCallback(ErrorCallback callback)
        {
            // Event 와의 결합 사용
            var error = CheckInitialized();
            if (error != null)
            {
                callback(error);
            }

            return initialized;
        }

        private bool CheckInitializedWithDataErrorCallback<T>(DataErrorCallback<T> callback) where T : class
        {
            var error = CheckInitialized();
            if (error != null)
            {
                callback(null, error);
            }

            return initialized;
        }

        private bool CheckInitializedWithProgressDelegate(ProgressEventCallback.ProgressDelegate callback)
        {
            var error = CheckInitialized();
            if (error != null)
            {
                callback(ProgressEventCallback.Type.OnStart, Result.Error(error));
            }

            return initialized;
        }

        private Error CheckInitialized()
        {
            if (initialized)
            {
                return null;
            }

            return new Error
            {
                code = ErrorCode.NOT_INITIALIZED.ToString(),
                message = "You must call Initialize"
            };
        }

        internal Configuration GetConfiguration()
        {
            return _configureManager.Configuration;
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
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}