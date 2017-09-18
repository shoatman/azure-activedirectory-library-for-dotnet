using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.Factories;
using Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.AuthorizationStrategies;
using Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.OAuthStrategies;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Controllers
{
    internal class AuthenticationContextController
    {
        private TokenCache _TokenCache; //Concrete Class (X)
        private Authenticator _Authenticator; //Concrete Class (X)
        private IdentityProviderFactory _IdentityProvider; //Abstract (Check)
        private CallState _CallState; //Concrete (X)

        internal AuthenticationContextController(TokenCache tokenCache, Authenticator authenticator)
        {
            //Needs parameter checking....

            this._TokenCache = tokenCache;
            this._Authenticator = authenticator;
            this._IdentityProvider = CreateIdentityProviderFactory();
            this._CallState = new CallState(new Guid());

            
        }

        private IdentityProviderFactory CreateIdentityProviderFactory()
        {
            switch (_Authenticator.AuthorityType)
            {
                case AuthorityType.AAD:
                    return new AzureActiveDirectoryIdentityProviderFactory();
                case AuthorityType.ADFS:
                    return new ADFSV3IdentityProviderFactory();
                default:
                    throw new ArgumentException();
            }
        }

        internal async Task<AuthenticationResult> AcquireTokenCommonAsync(string resource, string clientId,
            Uri redirectUri, IPlatformParameters parameters, UserIdentifier userId, string extraQueryParameters = null,
            string claims = null)
        {
            //RequestData requestData = new RequestData
            //{
            //    Authenticator = this._Authenticator,
            //    TokenCache = this._TokenCache,
            //    Resource = resource,
            //    ClientKey = new ClientKey(clientId),
            //    ExtendedLifeTimeEnabled = true,
            //};
            //var handler = new AcquireTokenInteractiveHandler(requestData, redirectUri, parameters, userId,
            //    extraQueryParameters, this.CreateWebAuthenticationDialog(parameters), claims);
            //return await handler.RunAsync().ConfigureAwait(false);
            CacheQueryData tokenQuery = new CacheQueryData() {
                Authority =this._Authenticator.Authority,
                Resource = resource,
                ClientId = clientId,
                UniqueId = userId.UniqueId,
                DisplayableId = userId.DisplayableId,
                SubjectType = TokenSubjectType.User,
            };

            //Note: Loading from the Token Cache modifies the result... it does the "new expiration" check and sets AccessToken to null if appropriate
            //Not sure How I feel about that... but don't love it

            AuthenticationResultEx result = GetTokenFromCache(tokenQuery);

            if (result?.Result != null &&
                ((result.Result.AccessToken == null && result.RefreshToken != null) ||
                    (result.Result.ExtendedLifeTimeToken && result.RefreshToken != null)))
            {
                //ResultEx = await this.RefreshAccessTokenAsync(ResultEx).ConfigureAwait(false);
                //result = await SendTokenRequestByRefreshTokenAsync(result.RefreshToken).ConfigureAwait(false);
                //if (result != null && result.Exception == null)
                //{
                //    //StoreResultExToCache(ref notifiedBeforeAccessCache);
                //    this._TokenCache.StoreToCache(result, this._Authenticator.Authority, resource,
                //    clientId, TokenSubjectType.User, this._CallState);
                //}
                throw new Exception("This code isn't working yet");
            }

            CodeAuthorizationRequest car = new CodeAuthorizationRequest {
                ClientId = clientId,
                RedirectUri = redirectUri,
                Scope = resource,
            };

            EmbeddedWebViewAuthorizationStrategy authStrategy = new EmbeddedWebViewAuthorizationStrategy();
            authStrategy.Context.PlatformParameters = parameters;

            AuthorizationResponse response = await this._IdentityProvider.CreateOAuthStrategy().RequestAuthorizationAsync(car, authStrategy);

            return new AuthenticationResult("asdf", "asd", new DateTimeOffset(DateTime.Now, TimeSpan.FromMinutes(55)));
        }

        private AuthenticationResultEx GetTokenFromCache(CacheQueryData query)
        {
            AuthenticationResultEx ResultEx = this._TokenCache.LoadFromCache(query, this._CallState);
            return ResultEx;
        }


    }
}
