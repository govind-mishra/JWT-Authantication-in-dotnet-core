using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiUsingCore.Domain
{
    public class RefreshToken
    {
        //refresh token 
        [Key]
        public string RToken { get; set; }

        //the given refresh token belongs to which jwttoken
        public string JWTToken { get; set; }

        public DateTime RTokenCreationDate { get; set; }

        public DateTime RTokenExpiryDate { get; set; }

        public bool isUsedToken { get; set; }

        public bool inValidated { get; set; }

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser user { get; set; }
    }
}
