using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiUsingCore.Contracts.v1.Requests
{
    public class RefreshTokenRequest
    {
        public string JWTToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
