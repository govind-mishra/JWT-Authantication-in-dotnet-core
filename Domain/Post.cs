using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiUsingCore.Domain
{
    public class Post
    {
        [Key]
        public Guid id { get; set; }
        public string Name { get; set; }

        //unique key for finding user
        public string userId { get; set; }

        //make relationship with userIdentity table as foreignkey
        [ForeignKey(nameof(userId))]
        public IdentityUser user { get; set; }


    }
}
