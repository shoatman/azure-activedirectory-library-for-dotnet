using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.OAuthStrategies
{
    internal class RefreshTokenTokenRequest : TokenRequest
    {
        public string RefreshToken { get; set; }
        public string Scope { get; set; }

    }
}
