using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrarCommon.Chat
{
    public class IncomingMessage
    {
        public string UserID { get; set; }
        public string SessionID { get; set; }
        public string DestinationUserID { get; set; }
        public string Text { get; set; }
    }
}
