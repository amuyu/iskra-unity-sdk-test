using Iskra.Service.Platforms;
using UnityEngine;

namespace Iskra.Service.Platforms.Mobile
{
    public class AndroidAuthTokenManager : IAuthTokenManager
    {
        private AndroidJavaClass pluginClass = null;
        private AndroidJavaObject pluginClassInstance = null;

        public bool Initialize()
        {
            using (pluginClass = new AndroidJavaClass("world.iskra.android.unity.plugin.IskraUnityPlugin"))
            {
                if (pluginClass != null)
                {
                    pluginClassInstance = pluginClass.CallStatic<AndroidJavaObject>("instance");
                    pluginClassInstance.Call("initialize");
                }
            }
            return pluginClass != null && pluginClassInstance != null;
        }

        public void StoreToken(string token)
        {
            pluginClassInstance.Call("storeAuthToken", token);
        }

        public string GetToken()
        {
            return pluginClassInstance.Call<string>("readAuthToken");
        }

        public void RemoveToken()
        {
            pluginClassInstance.Call("removeAuthToken");
        } 
    }
}