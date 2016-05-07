using System;
using System.Text;
using System.Threading.Tasks;

namespace RegistrarChatApi
{
    public class Connection
    {
        private readonly object _lock = new object();
        public string ConnectionID { get; set; }
        public string UserID { get; set; }
        public string SessionID { get; set; }
    }
}
