namespace Iskra.Service.Platforms
{
    public interface IAuthService
    {
        void SetUrls(string openWebUrl, string redirectUrl);
        void SignIn(string appId, AuthService.LoginCallback callback);
    }
}