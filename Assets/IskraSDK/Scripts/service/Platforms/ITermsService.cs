namespace Iskra.Service.Platforms
{
    public interface ITermsService
    {
        void SetUrls(string openWebUrl, string redirectUrl);
        void OpenWeb(string appId, string accessToken, TermsService.TermsViewCallback callback);

    }
}