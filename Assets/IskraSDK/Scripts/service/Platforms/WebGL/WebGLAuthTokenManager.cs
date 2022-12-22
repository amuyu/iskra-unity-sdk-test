using UnityEngine;

namespace Iskra.Service.Platforms.WebGL
{
    public class WebGLAuthTokenManager : IAuthTokenManager
    {
        private const string AUTH_TOKEN_KEY_PREFIX = "world.iskra.unity.plugin.accesstoken.";
        private string authTokenKey;

        public WebGLAuthTokenManager(string appId)
        {
            this.authTokenKey = AUTH_TOKEN_KEY_PREFIX + appId;
        }

        public bool Initialize()
        {
            return true;
        }

        public void StoreToken(string token)
        {
            PlayerPrefs.SetString(authTokenKey, token);
        }

        public string GetToken()
        {
            return PlayerPrefs.GetString(authTokenKey, null);
        }

        public void RemoveToken()
        {
            PlayerPrefs.DeleteKey(authTokenKey);
        }
    }
}