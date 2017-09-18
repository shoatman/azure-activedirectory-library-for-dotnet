using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.OAuthStrategies
{
    internal abstract class AuthorizationRequest
    {
        public string ResponseType { get; set; }

        public Uri AuthorizationUri { get; set; }

        public Dictionary<string, string> AdditionalParameters { get; set; }

    }
}
