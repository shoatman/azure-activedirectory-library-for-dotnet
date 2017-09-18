using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.OAuthStrategies
{
    internal abstract class TokenRequest
    {
        //This shouldn't be a string... better to strongly type
        public string GrantType { get; set; }

    }
}