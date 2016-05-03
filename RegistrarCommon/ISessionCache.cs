using System;

namespace RegistrarCommon
{
    public interface ISessionCache
    {
        event Action<Session> SessionExpired;
        void CloseSession(string sessionID);
        Session CreateSession(string userID);
        Session GetSessionByUserID(string userID);
        Session GetSessionBySessionID(string sessionID);
        bool VerifySession(string userID, string sessionID);
    }
}