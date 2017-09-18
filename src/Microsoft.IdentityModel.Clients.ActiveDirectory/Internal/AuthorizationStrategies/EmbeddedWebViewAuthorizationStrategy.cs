using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.OAuthStrategies;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.AuthorizationStrategies
{
    internal class EmbeddedWebViewAuthorizationStrategy : AuthorizationStrategy
    {

        public EmbeddedWebViewAuthorizationStrategy()
        {
            this.Context = new AuthorizationContext();
        }

        public async override Task<AuthorizationResult> AuthorizeAsync(AuthorizationRequest request)
        {
            if(request is CodeAuthorizationRequest)
            {
                return await this.AuthorizeAsync((CodeAuthorizationRequest)request);
            }
            else
            {
                throw new NotImplementedException();
            }
        }


        public async override Task<AuthorizationResult> AuthorizeAsync(CodeAuthorizationRequest request)
        {
            ActiveDirectory.AuthorizationResult result = await WebUIFactoryProvider.WebUIFactory.CreateAuthenticationDialog(Context.PlatformParameters).AcquireAuthorizationAsync(CreateRequestUri(request), request.RedirectUri, new CallState(new Guid()));

            return new AuthorizationResult() { Code = result.Code };
        }

        private Uri CreateRequestUri(CodeAuthorizationRequest request)
        {
            UriBuilder builder = new UriBuilder(request.AuthorizationUri);
            builder.Query += "?client_id=" + request.ClientId;
            builder.Query += "&resource=" + request.Scope;
            builder.Query += "&redirect_uri" + request.RedirectUri;
            return builder.Uri;
        }
    }
}
