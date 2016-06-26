using System;

namespace Common
{
    public interface ISessionCache
    {
        event Action<Session> SessionClosed;
        void CloseSession(string sessionID);
        Session CreateSession(string userID);
        Session GetSessionByUserID(string userID);
        Session GetSessionBySessionID(string sessionID);
        bool VerifySession(string userID, string sessionID);
    }
}