using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceParameters
{
    public static class Params
    {
        public const string BaseAddress = "http://as3.test1.doclogix.com/as3/identity";

        public const string TokenEndpoint = BaseAddress + "/connect/token";
        public const string IntrospectionEndpoint = BaseAddress + "/connect/introspect";
        public const string TokenRevocationEndpoint = BaseAddress + "/connect/revocation";

        public const string APIEndpoint = "http://api.test1.doclogix.com";
        public const string PrincipalController = "Principal";

    }

}
