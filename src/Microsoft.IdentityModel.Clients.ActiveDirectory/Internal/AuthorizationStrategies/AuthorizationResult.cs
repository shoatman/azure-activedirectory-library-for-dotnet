using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.AuthorizationStrategies
{
    internal class AuthorizationResult
    {
        public string Code { get; set; }

        public AuthorizationStatus Status { get; set; }

    }
}
