using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiUsingCore.Contracts.v1.Requests
{
    public class UserRegistrationRequest
    {
        [EmailAddress]//default MVC validator
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
