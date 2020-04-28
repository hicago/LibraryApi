using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Api.Models.Authorization
{
    public class LoginUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
