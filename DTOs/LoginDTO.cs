using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class LoginDTO
    {
        public required string UserName {get; set;}
        public required string Password {get; set;}
    }
}