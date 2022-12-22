using System.Collections;
using System.Collections.Generic;
using Iskra;
using Iskra.Service;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ProfileController : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI tokenText;

    public TextMeshProUGUI termsText;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("IskraAuth:" + IskraSDK.Instance.auth);
        nameText.text = "id:" + IskraSDK.Instance.auth.userId;
        GetWallet();
    }

    public void Signature()
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
                termsText.text = "agree" + data.agree;
                IskraSDK.Instance.ShowTermsView((data, error) =>
                {
                    if (error != null)
                    {
                        Debug.Log("error.code:" + error.code);
                        Debug.Log("error.message:" + error.message);
                    }
                    else
                    {
                        termsText.text = "agree" + data.agree;
                        Debug.Log("pass terms");
                    }
                });
            }
        });
    }

    public void ProgressCallback2(ProgressEventCallback.Type callbackType, Result result)
    {
        switch (callbackType)
        {
            case ProgressEventCallback.Type.OnStart:
                if (!result.status)
                {
                    // event 수신 실패
                }
                else
                {
                    // event 수신 준비 완료
                    // game server로 TX 전송 요청
                }
                break;
            case ProgressEventCallback.Type.OnFinish:
                if (result.error != null)
                {
                    Debug.Log("error.code:" + result.error.code);
                    Debug.Log("error.message:" + result.error.message);
                }
                else
                {
                    // 서명 및 트랜잭션 전송 완료
                }
                break;
            case ProgressEventCallback.Type.OnClose:
                // termsText.text = "onClose!!";
                Debug.LogFormat("Connection closed");
                break;
        }
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
                    termsText.text = "Failed to connect!!";
                }
                else
                {
                    termsText.text = "connected!!";
                    Debug.Log("Send message to server");
                }

                break;
            case ProgressEventCallback.Type.OnFinish:
                termsText.text = "finish!! " + result.status;
                Debug.Log("Finish signing and sendTransaction stauts:" + result.status);
                break;
            case ProgressEventCallback.Type.OnClose:
                // termsText.text = "onClose!!";
                Debug.LogFormat("Connection closed");
                break;
        }
    }
    
    public void GetWallet()
    {
        IskraSDK.Instance.GetWallet((wallet, error) =>
        {
            if (error != null)
            {
                Debug.Log("error.code:" + error.code);
                Debug.Log("error.message:" + error.message);
            }
            else
            {
                Debug.Log("walletAddress:" + wallet.walletAddress);
                tokenText.text = "walletAddress:" + wallet.walletAddress;
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
            else
            {
                Debug.Log("Logout !!");
                SceneManager.LoadScene("MobileLoginScene");
            }
        });
    }
}