using System.Runtime.InteropServices;

namespace Iskra.Service.Platforms.Mobile
{
    public class IOSAuthTokenManager : IAuthTokenManager
    {
  
        [DllImport("__Internal")]
        internal static extern void IskraPluginStoreAuthToken(string str);
        
        [DllImport("__Internal")]
        internal static extern string IskraPluginGetAuthToken();
        
        [DllImport("__Internal")]  
        internal static extern void IskraPluginRemoveAuthToken();

        public bool Initialize()
        {
            return true;
        }

        public void StoreToken(string token)
        {
            IskraPluginStoreAuthToken(token);
        }

        public string GetToken()
        {
            return IskraPluginGetAuthToken();
        }

        public void RemoveToken()
        {
            IskraPluginRemoveAuthToken();
        }
        
    }
}