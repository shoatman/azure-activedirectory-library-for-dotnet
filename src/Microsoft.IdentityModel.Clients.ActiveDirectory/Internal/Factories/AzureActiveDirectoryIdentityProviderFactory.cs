﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.OAuthStrategies;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.Factories
{
    internal class AzureActiveDirectoryIdentityProviderFactory : IdentityProviderFactory
    {
        public override OAuthStrategy CreateOAuthStrategy()
        {
            return new AzureActiveDirectoryOAuthStrategy();
        }
    }
}
