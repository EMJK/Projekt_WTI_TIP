using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrarWebApi.Models.Account
{
    public class RegisterAccountRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}