using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Ninject;
using RegistrarChatApiClient;
using RegistrarCommon;

namespace RegistrarChatApi
{
    public class ChatHub : Hub, IServerMethods
    {
        private readonly ISessionCache _cache;
        private readonly IList<Connection> _connections;

        public ChatHub(ISessionCache cache)
        {
            _cache = cache;
            _connections = RegistrarSignalRServer.Kernel.Get<IList<Connection>>();
            cache.SessionExpired += Cache_SessionExpired;
        }

        protected override void Dispose(bool disposing)
        {
            _cache.SessionExpired -= Cache_SessionExpired;
            base.Dispose(disposing);
        }

        private void Cache_SessionExpired(Session session)
        {
            lock (_connections)
            {
                var connection = _connections.FirstOrDefault(c => c.Session.SessionID == session.SessionID);
                if (connection != null)
                {
                    _connections.Remove(connection);
                    SendClientList();
                }
            }
        }

        private Session GetSession()
        {
            var con = _connections.FirstOrDefault(x => x.ConnectionID == Context.ConnectionId);
            if (con == null) return null;
            return _cache.GetSessionBySessionID(con.Session.SessionID);
        }

        public override Task OnConnected()
        {
            lock (_connections)
            {
                var userID = Context.QueryString["UserID"];
                var sessionID = Context.QueryString["SessionID"];
                if (_cache.VerifySession(userID, sessionID))
                {
                    var session = _cache.GetSessionBySessionID(sessionID);
                    _connections.Add(new Connection()
                    {
                        ConnectionID = Context.ConnectionId,
                        Session = session
                    });
                    SendClientList();
                }
            }
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            return OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            lock (_connections)
            {
                var connection = _connections.FirstOrDefault(c => c.ConnectionID == Context.ConnectionId);
                if (connection != null)
                {
                    _connections.Remove(connection);
                    SendClientList();
                }
            }
            return base.OnDisconnected(stopCalled);
        }

        private void SendClientList()
        {
            var msg = new ClientListParam()
            {
                Clients = _connections.Select(x => x.Session.UserID).ToList()
            };
            foreach (var connection in _connections)
            {
                Clients.Client(connection.ConnectionID).ClientList(msg);
            }
        }

        public void SendMessage(SendMessageParam param)
        {
            lock (_connections)
            {
                var session = GetSession();
                if (session != null)
                {
                    var destinationConnectionID = _connections.FirstOrDefault(x => x.Session.UserID == param.DestinationUserID)?.ConnectionID;
                    if (destinationConnectionID != null)
                    {
                        var msg = new MessageParam()
                        {
                            SenderUserID = session.UserID,
                            Message = param.Message
                        };
                        Clients.Client(destinationConnectionID).Message(msg);
                    }
                }
            }
        }
    }
}
