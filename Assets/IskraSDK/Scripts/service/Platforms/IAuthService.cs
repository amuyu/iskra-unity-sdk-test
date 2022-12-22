using UnityEditor;

namespace Iskra.Service.Platforms
{
    // android, ios, webGL
    public interface IAuthService
    {
        void Login(AuthService.LoginCallback callback);
        void Logout(AuthService.LogoutCallback callback);
        void StoreToken(Auth auth);
        Auth GetLastLogin();
        AuthService.LoginCallback GetLoginCallback();
    }
}