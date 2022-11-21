namespace Iskra.Service.Platforms
{
    public interface IWalletService
    {
        void SetUrls(string walletWebUrl, string redirectUrl);
        void OpenWallet(string appId, string accessToken, string data, string userId);
    }
}