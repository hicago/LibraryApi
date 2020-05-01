using Microsoft.AspNetCore.Identity;
using System;

namespace Library.Api.Entities
{
    public class User : IdentityUser
    {
        public DateTimeOffset BirthDate { get; set; }
    }

    public class Role : IdentityRole
    {

    }
}
