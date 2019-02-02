using System;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Entities
{
    public class User : IdentityUser
    {
        public virtual Employee Employee { get; set; }
    }
}