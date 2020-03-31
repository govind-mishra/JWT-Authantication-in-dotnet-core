using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiUsingCore.Contracts.v1.Responses
{
    public class AuthErrorResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}
