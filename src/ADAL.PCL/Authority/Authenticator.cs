//----------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
    internal enum AuthorityType
    {
        AAD,
        ADFS
    }

    [DataContract]
    internal sealed class InstanceDiscoveryResponse
    {
        [DataMember(Name = "tenant_discovery_endpoint")]
        public string TenantDiscoveryEndpoint { get; set; }

        [DataMember(Name = "preferred_network")]
        public string PreferredNetwork { get; set; }

        [DataMember(Name = "preferred_cache")]
        public string PreferredCache { get; set; }

        [DataMember(Name = "aliases")]
        public string[] Aliases { get; set; }
    }

    internal class Authenticator
    {
        public string DefaultTrustedAuthority = "login.microsoftonline.com";
        public HashSet<string> WhitelistedAuthorities = new HashSet<string>(new []
        {
            "login.windows.net",            // Microsoft Azure Worldwide - Used in validation scenarios where host is not this list 
            "login.chinacloudapi.cn",       // Microsoft Azure China
            "login.microsoftonline.de",     // Microsoft Azure Blackforest
            "login-us.microsoftonline.com", // Microsoft Azure US Government - Legacy
            "login.microsoftonline.us",     // Microsoft Azure US Government
            "login.microsoftonline.com"     // Microsoft Azure Worldwide
        });
        private const string AuthorizeEndpointTemplate = "https://{host}/{tenant}/oauth2/authorize";
        private const string TenantlessTenantName = "Common";

        private static readonly AuthenticatorTemplateList AuthenticatorTemplateList = new AuthenticatorTemplateList();

        private bool updatedFromTemplate; 

        public Authenticator(string authority, bool validateAuthority)
        {
            this.Authority = CanonicalizeUri(authority);

            this.AuthorityType = DetectAuthorityType(this.Authority);

            if (this.AuthorityType != AuthorityType.AAD && validateAuthority)
            {
                throw new ArgumentException(AdalErrorMessage.UnsupportedAuthorityValidation, "validateAuthority");
            }

            this.ValidateAuthority = validateAuthority;
        }

        public string Authority { get; private set; }

        public AuthorityType AuthorityType { get; private set; }

        public bool ValidateAuthority { get; private set; }

        public bool IsTenantless { get; private set; }

        public string AuthorizationUri { get; set; }

        public string DeviceCodeUri { get; set; }

        public string TokenUri { get; private set; }

        public string UserRealmUri { get; private set; }

        public string SelfSignedJwtAudience { get; private set; }

        public string PreferredCache { get; private set; }
        public string[] Aliases { get; private set; }

        public Guid CorrelationId { get; set; }

        public async Task<InstanceDiscoveryResponse> InstanceDiscoveryAsync(string discovererHost, string authorizeEndpoint, CallState callState)
        {
            string instanceDiscoveryEndpoint =
                ("https://{host}/common/discovery/instance?api-version=1.0&authorization_endpoint=" + authorizeEndpoint)
                    .Replace("{host}", discovererHost);
            var client = new AdalHttpClient(instanceDiscoveryEndpoint, callState);
            return await client.GetResponseAsync<InstanceDiscoveryResponse>().ConfigureAwait(false);
        }

        public async Task UpdateFromTemplateAsync(CallState callState)
        {
            if (!this.updatedFromTemplate)
            {
                var authorityUri = new Uri(this.Authority);
                string path = authorityUri.AbsolutePath.Substring(1);
                string tenant = path.Substring(0, path.IndexOf("/", StringComparison.Ordinal));
                InstanceDiscoveryResponse discoveryResponse = null;
                try // Always run instance discovery
                {
                    string instanceDiscoveryHost = WhitelistedAuthorities.Contains(authorityUri.Host) ? authorityUri.Host : DefaultTrustedAuthority;
                    string tentativeAuthorizeEndpoint = AuthorizeEndpointTemplate.Replace("{host}", authorityUri.Host).Replace("{tenant}", tenant);
                    discoveryResponse = await InstanceDiscoveryAsync(instanceDiscoveryHost, tentativeAuthorizeEndpoint, callState);
                    if (this.ValidateAuthority & discoveryResponse.TenantDiscoveryEndpoint == null)
                    { // hard stop here
                        throw new AdalException(AdalError.AuthorityNotInValidList);
                    }
                }
                catch (AdalServiceException ex)
                {
                    if (this.ValidateAuthority)
                    { // hard stop here
                        throw new AdalException((ex.ErrorCode == "invalid_instance") ? AdalError.AuthorityNotInValidList : AdalError.AuthorityValidationFailed, ex);
                    }
                }
                string preferredNetwork = discoveryResponse?.PreferredNetwork ?? authorityUri.Host;

                this.AuthorizationUri = AuthorizeEndpointTemplate.Replace("{host}", preferredNetwork).Replace("{tenant}", tenant);
                this.DeviceCodeUri = "https://{host}/{tenant}/oauth2/devicecode".Replace("{host}", preferredNetwork).Replace("{tenant}", tenant);
                this.TokenUri = "https://{host}/{tenant}/oauth2/token".Replace("{host}", preferredNetwork).Replace("{tenant}", tenant);
                this.UserRealmUri = CanonicalizeUri("https://{host}/common/UserRealm".Replace("{host}", preferredNetwork));
                this.IsTenantless = (string.Compare(tenant, TenantlessTenantName, StringComparison.OrdinalIgnoreCase) == 0);
                this.SelfSignedJwtAudience = this.TokenUri;
                this.updatedFromTemplate = true;
                this.PreferredCache = discoveryResponse?.PreferredCache ?? authorityUri.Host;
                this.Aliases = discoveryResponse?.Aliases;
            }
        }

        public void UpdateTenantId(string tenantId)
        {
            if (this.IsTenantless && !string.IsNullOrWhiteSpace(tenantId))
            {
                this.ReplaceTenantlessTenant(tenantId);
                // this.updatedFromTemplate = false;  // ???
            }
        }

        internal static AuthorityType DetectAuthorityType(string authority)
        {
            if (string.IsNullOrWhiteSpace(authority))
            {
                throw new ArgumentNullException("authority");
            }

            if (!Uri.IsWellFormedUriString(authority, UriKind.Absolute))
            {
                throw new ArgumentException(AdalErrorMessage.AuthorityInvalidUriFormat, "authority");
            }

            var authorityUri = new Uri(authority);
            if (authorityUri.Scheme != "https")
            {
                throw new ArgumentException(AdalErrorMessage.AuthorityUriInsecure, "authority");
            }

            string path = authorityUri.AbsolutePath.Substring(1);
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(AdalErrorMessage.AuthorityUriInvalidPath, "authority");
            }

            string firstPath = path.Substring(0, path.IndexOf("/", StringComparison.Ordinal));
            AuthorityType authorityType = IsAdfsAuthority(firstPath) ? AuthorityType.ADFS : AuthorityType.AAD;

            return authorityType;
        }

        private static string CanonicalizeUri(string uri)
        {
            if (!string.IsNullOrWhiteSpace(uri) && !uri.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                uri = uri + "/";
            }

            return uri;
        }

        private static bool IsAdfsAuthority(string firstPath)
        {
            return string.Compare(firstPath, "adfs", StringComparison.OrdinalIgnoreCase) == 0;
        }

        private void ReplaceTenantlessTenant(string tenantId)
        {
            var regex = new Regex(Regex.Escape(TenantlessTenantName), RegexOptions.IgnoreCase);
            this.Authority = regex.Replace(this.Authority, tenantId, 1);
        }
    }
}
