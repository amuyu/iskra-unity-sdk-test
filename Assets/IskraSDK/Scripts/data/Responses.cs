using System;

namespace Iskra
{
    [Serializable]
    public class ConfigureResponse
    {
        public UserConfigureResponse user;
        public WalletConfigureResponse wallet;
    }

    [Serializable]
    public class UserConfigureResponse
    {
        public string loginPageUrl;
        public string loginSuccessUrl;
        public string verifyUrl;
        public string termsPageUrl;
        public string termsSuccessUrl;
        public string termsVerifyUrl;
    }

    [Serializable]
    public class WalletConfigureResponse
    {
        public string signaturePageUrl;
        public string signatureSuccessUrl;
        public string connectUrl;
    }
    
    [Serializable]
    public class TermsAgreeResponse
    {
        public bool agree;

        public Terms toTerms()
        {
            return new Terms
            {
                agree = agree
            };
        }
    }
}