using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrarCommon.Chat
{
    public class RegisterRequest
    {
        public string UserID { get; set; }
        public string SessionID { get; set; }
    }

    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string StatusMessage { get; set; }
    }
}
