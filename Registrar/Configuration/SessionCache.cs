using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;

namespace Server
{
    class SessionCache : ISessionCache
    {
        private readonly object _lockObj = new object();
        private Timer _timer;
        private readonly List<Session> _sessions; 
        private readonly TimeSpan _sessionLifetime;
        public event Action<Session> SessionClosed;

        public SessionCache(TimeSpan sessionLifetime)
        {
            _sessionLifetime = sessionLifetime;
            _sessions = new List<Session>();
            _timer = new Timer(Elapsed, null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromSeconds(1));
        }

        private void Elapsed(object state)
        {
            lock (_lockObj)
            {
                var now = DateTime.Now;
                foreach (var session in _sessions.ToList())
                {
                    if (session.LastRefreshed + _sessionLifetime < now)
                    {
                        SessionClosed?.Invoke(session);
                        _sessions.Remove(session);
                    }
                    else
                    {
                        session.LastRefreshed = now;
                    }
                }
            }
        }

        public Session CreateSession(string userID)
        {
            var session = new Session(userID, Guid.NewGuid().ToString("N"), DateTime.Now);
            lock (_lockObj)
            {
                _sessions.Add(session);
            }
            return session;
        }

        public Session GetSessionByUserID(string userID)
        {
            lock (_lockObj)
            {
                return _sessions.FirstOrDefault(s => s.UserID == userID);
            }
        }

        public Session GetSessionBySessionID(string sessionID)
        {
            lock (_lockObj)
            {
                return _sessions.FirstOrDefault(s => s.SessionID == sessionID);
            }
        }

        public void CloseSession(string sessionID)
        {
            lock (_lockObj)
            {
                var sessionToRemove = _sessions.FirstOrDefault(s => s.SessionID == sessionID);
                if (sessionToRemove != null)
                {
                    _sessions.Remove(sessionToRemove);
                    SessionClosed?.Invoke(sessionToRemove);
                }
            }
        }

        public bool VerifySession(string userID, string sessionID)
        {
            lock (_lockObj)
            {
                var session = _sessions.FirstOrDefault(s => s.UserID == userID && s.SessionID == sessionID);
                if (session != null)
                {
                    session.LastRefreshed = DateTime.Now;
                    return true;
                }
                return false;
            }
        }
    }
}
