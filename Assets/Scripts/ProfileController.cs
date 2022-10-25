using System.Collections;
using System.Collections.Generic;
using Iskra;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ProfileController : MonoBehaviour
{
    
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI tokenText;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("IskraAuth:" + IskraSDK.Instance.auth);
        nameText.text = "id:" + IskraSDK.Instance.auth.userId;
        tokenText.text = "token:" + IskraSDK.Instance.auth.accessToken;
        // StartCoroutine(fetchMe()); 
    }

}
