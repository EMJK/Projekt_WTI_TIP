using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace RegistrarCommon
{
    public class SessionCache : ISessionCache
    {
        private readonly TimeSpan _sessionLifetime;
        private readonly MemoryCache _cache;
        public event Action<Session> SessionExpired;

        public SessionCache(TimeSpan sessionLifetime)
        {
            _sessionLifetime = sessionLifetime;
            _cache = new MemoryCache("SessionCache");
        }

        public Session CreateSession(string userID)
        {
            var session = new Session(userID, Guid.NewGuid().ToString("N"), DateTimeOffset.Now);
            var cacheItem = new CacheItem(userID, session);
            var policy = new CacheItemPolicy()
            {
                SlidingExpiration = _sessionLifetime,
                UpdateCallback = UpdateCallback
            };
            _cache.Set(cacheItem, policy);
            return session;
        }

        public Session GetSession(string userID)
        {
            return (Session) _cache.Get(userID);
        }

        public void CloseSession(string userID)
        {
            _cache.Remove(userID);
        }

        public bool VerifySession(string userID, string sessionID)
        {
            return ((Session) _cache.Get(userID))?.SessionID == sessionID;
        }

        private void UpdateCallback(CacheEntryUpdateArguments arguments)
        {
            OnSessionExpired((Session) arguments.UpdatedCacheItem.Value);
        }

        protected virtual void OnSessionExpired(Session session)
        {
            SessionExpired?.Invoke(session);
        }
    }
}
