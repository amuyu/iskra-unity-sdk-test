using UnityEngine;

namespace DefaultNamespace
{
    public class AnroidUnityPlugin
    {
        private AndroidJavaClass pluginClass = null;
        private AndroidJavaObject pluginClassInstance = null; 
        
        public bool Initialize()
        {
            using (pluginClass = new AndroidJavaClass("world.iskra.android.unity.plugin.IskraUnityPlugin"))
            {
                if (pluginClass != null)
                {
                    //아까 싱글톤으로 사용하자고 만들었던 static instance를 불러와줍니다.
                    pluginClassInstance = pluginClass.CallStatic<AndroidJavaObject>("instance");
                    //Context를 설정해줍니다.
                    pluginClassInstance.Call("initialize");
                }
            }

            return (pluginClass != null && pluginClassInstance != null);
        }
        
        public void StoreToken(string token)
        {
            pluginClassInstance.Call("storeAuthToken", token);
        }
        
        public string GetToken()
        {
            return pluginClassInstance.Call<string>("readAuthToken");
        }
    }
    

    
   
}