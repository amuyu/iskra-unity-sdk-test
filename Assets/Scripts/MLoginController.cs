using System;
using System.Web;
using Iskra;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MLoginController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var appId = "EalSEwkoUHgMuSOYqxoB";
        var phase = "dev";
        IskraSDK.Instance.Initialize(phase, appId, error =>
        {
            if (error != null)
            {
                Debug.Log("error.code:" + error.code);
                Debug.Log("error.message:" + error.message);
            }
            else
            {
                Debug.Log("success initialize");
                // IskraSDK.Instance.SetConfiguration(configuration);
            }
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
 

    public void MobileSignIn()
    {
        Debug.Log("########### MobileSignIn!!!!");
        IskraSDK.Instance.Login((data, error) =>
        {
            if (error != null)
            {
                Debug.Log("error.code:" + error.code);
                Debug.Log("error.message:" + error.message);
            }
            else
            {
                Auth.userId = data.userId;
                Auth.accessToken = data.accessToken;
                Debug.Log("userId:" + data.userId);
                Debug.Log("walletAddress:" + data.walletAddress);
                Debug.Log("accessToken:" + data.accessToken);
                Debug.Log("refreshToken:" + data.refreshToken);
                SceneManager.LoadScene("ProfileScene");
            }
        });
    }
    
    
}
