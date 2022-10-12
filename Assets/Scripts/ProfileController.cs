using System.Collections;
using System.Collections.Generic;
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
        nameText.text = "id:" + Auth.userId;
        tokenText.text = "token:" + Auth.accessToken;
        // StartCoroutine(fetchMe()); 
    }

}
