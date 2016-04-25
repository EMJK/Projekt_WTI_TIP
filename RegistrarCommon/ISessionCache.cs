using System;

namespace RegistrarCommon
{
    public interface ISessionCache
    {
        event Action<Session> SessionExpired;

        void CloseSession(string userID);
        Session CreateSession(string userID);
        Session GetSession(string userID);
        bool VerifySession(string userID, string sessionID);
    }
}