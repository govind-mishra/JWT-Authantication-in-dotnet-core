using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiUsingCore.Domain
{
    public class AuthanticationResult
    {
        public string JWTToken { get; set; }
        public bool Success { get; set; }

        public string RefreshToken { get; set; }

        public IEnumerable<string> Error { get; set; }
    }
}
