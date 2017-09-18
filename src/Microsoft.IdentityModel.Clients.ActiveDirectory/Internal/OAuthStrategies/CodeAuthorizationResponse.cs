﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.OAuthStrategies
{
    internal class CodeAuthorizationResponse : AuthorizationResponse
    {
        public string Code { get; set; }
    }
}
