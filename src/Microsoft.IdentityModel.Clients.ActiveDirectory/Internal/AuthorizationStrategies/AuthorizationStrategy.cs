using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.OAuthStrategies;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.AuthorizationStrategies
{
    internal abstract class AuthorizationStrategy
    {
        public AuthorizationContext Context { get; set; }

        public abstract Task<AuthorizationResult> AuthorizeAsync(AuthorizationRequest request);

        public abstract Task<AuthorizationResult> AuthorizeAsync(CodeAuthorizationRequest request);
    }
}
