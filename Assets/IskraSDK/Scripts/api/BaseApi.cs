using System;
using System.Collections;
using Iskra.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace Iskra.Api
{
    public class BaseApi : MonoBehaviour
    {
        private static BaseApi instance = null;
        public static BaseApi Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject("IskraBaseApi");
                    instance = obj.AddComponent<BaseApi>();
                    DontDestroyOnLoad(obj);
                }
                return instance;
            }
        }

        public void Send(IEnumerator routine)
        {
            StartCoroutine(routine);
        }

        public static Error HandleHttpError(UnityWebRequest request)
        {
            Error error = null;
            if (request.isNetworkError || request.isHttpError)
            {
                try
                {
                    error = JsonUtility.FromJson<Error>(request.downloadHandler.text);
                    if (string.IsNullOrEmpty(error.code) || string.IsNullOrEmpty(error.message))
                    {
                        error = new Error
                        {
                            code = ErrorCode.SERVER_ERROR.ToString(),
                            message = request.error
                        };
                    }
                }
                catch (Exception e)
                {
                    error = new Error
                    {
                        code = ErrorCode.INTERNAL_ERROR.ToString(),
                        message = e.Message
                    };
                }
            }
            return error;
        }
        
    }
}