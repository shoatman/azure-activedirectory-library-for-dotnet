using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.OAuthStrategies
{
    internal class CodeAuthorizationRequest : AuthorizationRequest
    {
        public string ClientId { get; set; }
        public Uri RedirectUri { get; set; }
        public string Scope { get; set; }
        public string State { get; set; }

    }
}
