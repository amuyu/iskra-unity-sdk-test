namespace Iskra.Service.Platforms
{
    public interface ITermsService
    {
        void OpenWeb(string accessToken, TermsService.TermsViewCallback callback);
        TermsService.TermsViewCallback GetTermsViewCallback();

    }
}