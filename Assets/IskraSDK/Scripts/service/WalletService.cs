#if UNITY_ANDROID || UNITY_IOS
using Iskra.Service.Platforms.Mobile;
#elif UNITY_WEBGL
using Iskra.Service.Platforms.WebGL;
#endif
using Iskra.Service.Platforms;

namespace Iskra.Service
{
    public class WalletService : IWalletService
    {
        public IWalletService walletService;

        public WalletService()
        {
#if UNITY_ANDROID || UNITY_IOS
            walletService = new MobileWalletService();
#elif UNITY_WEBGL
            walletService = new WebGLWalletService();
#endif
        }

        private static WalletService instance;

        public static WalletService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WalletService();
                }

                return instance;
            }
        }


        public void SetUrls(string walletWebUrl, string redirectUrl)
        {
            walletService.SetUrls(walletWebUrl, redirectUrl);
        }

        public void OpenWallet(string appId, string accessToken, string data, string userId)
        {
            walletService.OpenWallet(appId, accessToken, data, userId);
        }
    }
}