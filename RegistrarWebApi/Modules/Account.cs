using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace RegistrarWebApi.Modules
{
    public class Account : ModuleBase
    {
        public string StartSession(string name)
        {
            var sessionID = Guid.NewGuid().ToString("N");
            return $"Hello {name}! Your new session ID is {sessionID}.";
        }
    }
}
