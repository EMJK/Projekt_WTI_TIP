using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrarCommon
{
    public class Session
    {
        public DateTimeOffset StartedAt { get; }
        public string UserID { get; }
        public string SessionID { get; }

        public Session(string userID, string sessionID, DateTimeOffset startedAt)
        {
            UserID = userID;
            SessionID = sessionID;
            StartedAt = startedAt;
        }
    }
}
