using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Iskra
{
    public class Auth 
    {
        public string userId;
        public string accessToken;
        public string refreshToken;
        public string walletAddress;

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(accessToken);
        }

        public void UpdateToken(
            string accessToken,
            string refreshToken)
        {
            this.accessToken = accessToken;
            this.refreshToken = refreshToken;
        }
    }
}

