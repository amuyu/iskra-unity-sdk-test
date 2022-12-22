namespace Iskra.Service.Platforms
{
    public interface IAuthTokenManager
    {
        bool Initialize();
        void StoreToken(string token);
        string GetToken();
        void RemoveToken();
    }
}