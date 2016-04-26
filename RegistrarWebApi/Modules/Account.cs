using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using RegistrarWebApi.Models.Account;

namespace RegistrarWebApi.Modules
{
    public class Account : ModuleBase
    {
        public RegisterAccountResponse Register(RegisterAccountRequest data)
        {
            return new RegisterAccountResponse()
            {
                Message = $"A new account for \"{data.UserName}\" was created."
            };
        }


    }
}
