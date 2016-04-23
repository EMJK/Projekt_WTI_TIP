using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace RegistrarWebApi.Modules
{
    public class Account : ModuleBase
    {
        public string Echo(string name)
        {
            return $"Hello {name ?? "null"}!";
        }

        public string EchoNumber(int number)
        {
            return $"Hello {number}!";
        }
    }
}
