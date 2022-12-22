using DefaultNamespace;
using Iskra;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Iskra.Common.Logger;

public class MLoginController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Initialize()
    {
        // var appId = "EalSEwkoUHgMuSOYqxoB";
        var appId = "ho3gx52pKd0Emi21IQ1C";
        var phase = "dev";
        Logger.EnableLog();
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

    public void LastLogin()
    {
        IskraSDK.Instance.LoadLastLogin((data, error) =>
        {
            if (error != null)
            {
                Debug.Log("error.code:" + error.code);
                Debug.Log("error.message:" + error.message);
                // login
            }
            else
            {
                Debug.Log("userId:" + data.userId);
                Debug.Log("accessToken:" + data.accessToken);
                Debug.Log("refreshToken:" + data.refreshToken); 
                
                IskraSDK.Instance.VerifyAccessToken(error =>
                {
                    if (error != null)
                    {
                        Debug.Log("error.code:" + error.code);
                        Debug.Log("error.message:" + error.message);
                    }
                    else
                    {
                        Debug.Log("Successed verify");
                        SceneManager.LoadScene("ProfileScene");
                    }
                });
                
            }
        });
        
    }

    public void Logout()
    {
        IskraSDK.Instance.Logout(error =>
        {
            if (error != null)
            {
                Debug.Log("error.code:" + error.code);
                Debug.Log("error.message:" + error.message);
            }
        });
    }

    public void RefreshToken()
    {
        Debug.Log("RefreshToken!!");
        IskraSDK.Instance.RefreshAccessToken((data, error) =>
        {
            if (error != null)
            {
                Debug.Log("error.code:" + error.code);
                Debug.Log("error.message:" + error.message);
            }
            else
            {
                Debug.Log("Successed Refresh");
            }
        });
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
                Debug.Log("userId:" + data.userId);
                Debug.Log("accessToken:" + data.accessToken);
                Debug.Log("refreshToken:" + data.refreshToken);
                SceneManager.LoadScene("ProfileScene");
            }
        });
    }
    
}