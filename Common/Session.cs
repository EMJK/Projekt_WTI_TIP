using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Session
    {
        public DateTime StartedAt { get; }
        public DateTime LastRefreshed { get; set; }
        public string UserID { get; }
        public string SessionID { get; }

        public Session(string userID, string sessionID, DateTime startedAt)
        {
            UserID = userID;
            SessionID = sessionID;
            StartedAt = startedAt;
            LastRefreshed = startedAt;
        }
    }
}
