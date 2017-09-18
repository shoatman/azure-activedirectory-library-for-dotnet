using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.OAuthStrategies
{
    internal class AuthorizationCodeTokenRequest : TokenRequest
    {
        public string Code { get; set; }
        public string RedirectUri { get; set; }
        public string ClientId { get; set; }
    }
}
