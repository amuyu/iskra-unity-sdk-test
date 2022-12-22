#if UNITY_ANDROID || UNITY_IOS
using Iskra.Service.Platforms.Mobile;
#elif UNITY_WEBGL
using Iskra.Service.Platforms.WebGL;
#endif
using System;
using System.Collections;
using Iskra.Api;
using Iskra.Common;
using Iskra.Service.Platforms;
using UnityEngine;
using UnityEngine.Networking;
using Logger = Iskra.Common.Logger;

namespace Iskra.Service
{
    public class AuthService
    {
        public IAuthService authService;

        public delegate void LoginCallback(Auth data, Error error);

        public delegate void LogoutCallback(Error error);

        public delegate void VerifyAccessTokenCallback(Error error);

        public delegate void GetWalletCallback(Wallet wallet, Error error);

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

        // WebGL
        public void Login(LoginCallback callback)
        {
            authService.Login(callback);
        }

        public void VerifyAccessToken(
            string accessToken,
            VerifyAccessTokenCallback callback)
        {
            BaseApi.Instance.Send(VerifyAccessTokenRoutine(
                accessToken, callback));
        }

        public void RefreshAccessToken(
            string accessToken,
            string refreshToken,
            LoginCallback callback)
        {
            BaseApi.Instance.Send(RefreshAccessTokenRoutine(
                accessToken, refreshToken, callback));
        }


        public void GetWallet(string accessToken, GetWalletCallback callback)
        {
            BaseApi.Instance.Send(VerifyAccessTokenRoutine(
                accessToken, (error) =>
                {
                    if (error != null)
                    {
                        callback(null, error);
                    }
                    else
                    {
                        var wallet = IskraSDK.Instance.wallet;
                        if (wallet == null || string.IsNullOrEmpty(wallet.walletAddress))
                        {
                            callback(null, new Error
                            {
                                code = ErrorCode.NOT_FOUND_WALLET.ToString(),
                                message = "Cannot find wallet."
                            });
                        }
                        else
                        {
                            callback(IskraSDK.Instance.wallet, null);
                        }
                    }
                }));
        }

        public void Logout(LogoutCallback callback)
        {
            authService.Logout(callback);
        }

        public void LoadLastLogin(LoginCallback callback)
        {
            var auth = GetLastLogin();
            if (auth == null || auth.IsEmpty())
            {
                callback(null, new Error
                {
                    code = ErrorCode.NOT_LOGGED_IN.ToString(),
                    message = "Cannot find login information."
                });
                return;
            }

            // Verify 
            VerifyAccessToken(auth.accessToken, error =>
            {
                if (error != null)
                {
                    Logger.Debug(error.ToString(), this);
                    // 토큰 만료인 경우, RefreshToken
                    if (error.code == ErrorCode.EXPIRED_ACCESS_TOKEN.ToString())
                    {
                        RefreshAccessToken(auth.accessToken, auth.refreshToken, callback);
                        return;
                    }
                }

                callback(auth, error);
            });
        }

        public Auth GetLastLogin()
        {
            return authService.GetLastLogin();
        }

        public IEnumerator VerifyAccessTokenRoutine(string accessToken,
            VerifyAccessTokenCallback callback)
        {
            var configuration = IskraSDK.Instance.GetConfiguration();
            var downloadHandler = new DownloadHandlerBuffer();
            var uploadHandler = new UploadHandlerRaw(null);

            using (var request = new UnityWebRequest(configuration.verifyUrl, UnityWebRequest.kHttpVerbGET,
                       downloadHandler, uploadHandler))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("X-Iskra-App-Id", configuration.appId);
                request.SetRequestHeader("Authorization", "Bearer " + accessToken);

                yield return request.SendWebRequest();

                var error = BaseApi.HandleHttpError(request);
                if (error != null)
                {
                    callback(error);
                }
                else
                {
                    var response = JsonUtility.FromJson<VerifyAccessTokenResponse>(request.downloadHandler.text);
                    IskraSDK.Instance.wallet = new Wallet
                    {
                        walletAddress = response.walletAddress
                    };
                    callback(null);
                }
            }
        }

        public IEnumerator RefreshAccessTokenRoutine(
            string accessToken,
            string refreshToken,
            LoginCallback callback)
        {
            var configuration = IskraSDK.Instance.GetConfiguration();
            var downloadHandler = new DownloadHandlerBuffer();
            var body = JsonUtility.ToJson(new RefreshTokenRequest
            {
                refreshToken = refreshToken
            });
            byte[] rawBody = new System.Text.UTF8Encoding().GetBytes(body);
            var uploadHandler = new UploadHandlerRaw(rawBody);

            using (var request =
                   new UnityWebRequest(configuration.refreshTokenUrl, UnityWebRequest.kHttpVerbPOST, downloadHandler,
                       uploadHandler))
            {
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
                    var response = JsonUtility.FromJson<RefreshAccessTokenResponse>(request.downloadHandler.text);
                    IskraSDK.Instance.auth.UpdateToken(response.accessToken, response.refreshToken);
                    StoreToken(IskraSDK.Instance.auth);
                    callback(IskraSDK.Instance.auth, null);
                }
            }
        }

        private void StoreToken(Auth auth)
        {
            authService.StoreToken(auth);
        }
    }
}