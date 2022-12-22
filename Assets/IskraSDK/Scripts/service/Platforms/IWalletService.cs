namespace Iskra.Service.Platforms
{
    public interface IWalletService
    {
        void OpenWallet(string accessToken, string data, string userId, WalletService.OpenWalletCallback callback);

        WalletService.OpenWalletCallback GetOpenWalletCallback();
    }
}