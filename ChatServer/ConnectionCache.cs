using System.Collections.Generic;
using System.Linq;
using Julas.Utils.Extensions;
using Common;

namespace ChatServer
{
    public class ConnectionCache : IConnectionCache
    {
        private readonly List<Connection> _connections = new List<Connection>();
        private readonly ISessionCache _sessions;

        public ConnectionCache(ISessionCache sessions)
        {
            _sessions = sessions;
        }

        public bool Verify(Connection con)
        {
            if (con == null ||
                con.SessionID.IsNullOrWhitespace() || 
                con.ConnectionID.IsNullOrWhitespace() ||
                con.UserID.IsNullOrWhitespace())
            {
                return false;
            }
            lock (_connections)
            {
                _connections.RemoveAll(c =>
                    c.SessionID.ToLower() == con.SessionID.ToLower() ||
                    c.ConnectionID.ToLower() == con.ConnectionID.ToLower() ||
                    c.UserID.ToLower() == con.UserID.ToLower());
                if (!_sessions.VerifySession(con.UserID.ToLower(), con.SessionID.ToLower()))
                {
                    return false;
                }
                _connections.Add(con);
                return true;
            }
        }

        public void RemoveConnectionForSessionID(string sessionID)
        {
            lock (_connections)
            {
                _connections.RemoveAll(c => c.SessionID.ToLower() == sessionID.ToLower());
            }
        }

        public void RemoveConnectionForConnectionID(string connectionID)
        {
            lock (_connections)
            {
                _connections.RemoveAll(c => c.ConnectionID.ToLower() == connectionID.ToLower());
            }
        }

        public List<Connection> GetValidConnections()
        {
            lock (_connections)
            {
                var ret = new List<Connection>();
                foreach (var con in _connections.ToList())
                {
                    if (Verify(con))
                    {
                        ret.Add(con);
                    }
                }
                return ret;
            }
        } 

        public string GetConnectionIDForUser(string userID)
        {
            lock (_connections)
            {
                return _connections.FirstOrDefault(c => c.UserID.ToLower() == userID.ToLower())?.ConnectionID;
            }
        }

        public string GetConnectionIDForSession(string sessionID)
        {
            lock (_connections)
            {
                return _connections.FirstOrDefault(c => c.SessionID.ToLower() == sessionID.ToLower())?.ConnectionID;
            }
        }
    }
}