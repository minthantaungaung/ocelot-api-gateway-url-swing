using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthenticationManager.Models
{
    public class UserAccount
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class TokenUserAccount
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ProjectName { get; set; }
        public string Role { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }        
    }

}
