namespace Iskra
{
    public class Configuration
    {
        public readonly bool initialized;
        public readonly string phase;
        public readonly string appId;
        public readonly string authUrl;
        public readonly string authRedirectUrl;
        public readonly string verifyUrl;
        public readonly string refreshTokenUrl;
        public readonly string termsUrl;
        public readonly string termsRedirectUrl;
        public readonly string termsVerifyUrl;
        public readonly string walletUrl;
        public readonly string walletRedirectUrl;
        public readonly string websocketUrl;

        public Configuration(bool initialized, string phase, string appId, string authUrl, string authRedirectUrl,
            string verifyUrl, string refreshTokenUrl, string termsUrl, string termsRedirectUrl, string termsVerifyUrl,
            string walletUrl, string walletRedirectUrl, string websocketUrl)
        {
            this.phase = phase;
            this.appId = appId;
            this.authUrl = authUrl;
            this.authRedirectUrl = authRedirectUrl;
            this.verifyUrl = verifyUrl;
            this.refreshTokenUrl = refreshTokenUrl;
            this.termsUrl = termsUrl;
            this.termsRedirectUrl = termsRedirectUrl;
            this.termsVerifyUrl = termsVerifyUrl;
            this.walletUrl = walletUrl;
            this.walletRedirectUrl = walletRedirectUrl;
            this.websocketUrl = websocketUrl;
            this.initialized = initialized;
        }
    }
}