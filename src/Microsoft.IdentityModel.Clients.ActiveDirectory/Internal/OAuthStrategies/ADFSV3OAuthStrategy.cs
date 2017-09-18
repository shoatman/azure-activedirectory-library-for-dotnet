using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.OAuthStrategies
{
    internal class ADFSV3OAuthStrategy : OAuthStrategy
    {
        protected override void ValidateAuthorizationRequest(AuthorizationRequest request)
        {
            throw new NotImplementedException();
        }

        protected override void ValidateTokenRequest(TokenRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
