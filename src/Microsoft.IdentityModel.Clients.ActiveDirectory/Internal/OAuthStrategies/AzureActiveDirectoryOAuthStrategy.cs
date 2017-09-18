using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.OAuthStrategies
{
    internal class AzureActiveDirectoryOAuthStrategy : OAuthStrategy
    {
        public AzureActiveDirectoryOAuthStrategy()
        {
            this._AuthorizationEndpoint = @"/oauth2/authorize";
            this._TokenEndpoint = @"/oauth2/token";
            this._Issuer = new Uri("https://login.microsoftonline.com/common");
        }

        protected override void ValidateAuthorizationRequest(AuthorizationRequest request)
        {
            return;
        }

        protected override void ValidateTokenRequest(TokenRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
