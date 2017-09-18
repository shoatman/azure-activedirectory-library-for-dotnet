using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.AuthorizationStrategies;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.OAuthStrategies
{
    internal abstract class OAuthStrategy
    {

        protected string _TokenEndpoint;
        protected string _AuthorizationEndpoint;
        protected Uri _Issuer;

        public Task<TokenResponse> RequestTokenAsync(TokenRequest request)
        {
            throw new NotImplementedException();
        }
        public async Task<AuthorizationResponse> RequestAuthorizationAsync(AuthorizationRequest request, AuthorizationStrategy authStrategy)
        {
            ValidateAuthorizationRequest(request);
            request.AuthorizationUri = CreateAuthorizationUri();
            AuthorizationStrategies.AuthorizationResult result = await authStrategy.AuthorizeAsync(request);
            CodeAuthorizationResponse response = new CodeAuthorizationResponse();
            response.Code = result.Code;
            return response;
        }

        protected virtual Uri CreateAuthorizationUri()
        {
            UriBuilder builder = new UriBuilder(_Issuer);
            builder.Path += _AuthorizationEndpoint;
            return builder.Uri;
        }

        protected abstract void ValidateAuthorizationRequest(AuthorizationRequest request);
        protected abstract void ValidateTokenRequest(TokenRequest request);
       
    }
}
