using System.Collections.Generic;

namespace ChatServer
{
    public interface IConnectionCache
    {
        string GetConnectionIDForSession(string sessionID);
        string GetConnectionIDForUser(string userID);
        bool Verify(Connection con);
        void RemoveConnectionForSessionID(string sessionID);
        void RemoveConnectionForConnectionID(string conenctionID);
        List<Connection> GetValidConnections();
    }
}