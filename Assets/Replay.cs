using System.Collections;
using System.Collections.Generic;
using Iskra;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Replay : MonoBehaviour
{
    public void ReplayGame()
    {
        IskraSDK.Instance.PrepareSigning(ProgressCallback);
    }

    public void GetTermsAgree()
    {
        IskraSDK.Instance.GetTermsAgree((data, error) =>
        {
            if (error != null)
            {
                Debug.Log("error.code:" + error.code);
                Debug.Log("error.message:" + error.message);
            }
            else
            {
                Debug.Log("agree:" + data.agree);
                if (!data.agree)
                {
                    IskraSDK.Instance.ShowTermsView((data, error) =>
                    {
                        if (error != null)
                        {
                            Debug.Log("error.code:" + error.code);
                            Debug.Log("error.message:" + error.message);
                        }
                        else
                        {
                            Debug.Log("pass terms");
                        }
            
                    });
                }
            }
            
        });
    }
    
    public void ProgressCallback(ProgressEventCallback.Type callbackType, Result result)
    {
        Debug.Log("OnCallback: " + callbackType);
        switch (callbackType)
        {
            case ProgressEventCallback.Type.OnStart:
                if (!result.status)
                {
                    Debug.LogFormat("Fail to connect transaction service. Error:{0}", result.error);
                }
                else
                {
                    Debug.Log("Send message to server");
                }
                break;
            case ProgressEventCallback.Type.OnFinish:
                Debug.Log("Finish signing and sendTransaction stauts:" + result.status);
                break;
            case ProgressEventCallback.Type.OnClose:
                Debug.LogFormat("Connection closed");
                break;
        }
    }
}
