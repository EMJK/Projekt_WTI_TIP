using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RegistrarCommon;
using RegistrarCommon.Chat;

namespace RegistrarChatApi
{
    public class ChatHub : Hub
    {
        private readonly ISessionCache _cache;
        private readonly List<Connection> _connections; 
        private readonly object _lock = new object();

        public ChatHub(ISessionCache cache)
        {
            _cache = cache;
            _connections = new List<Connection>();
            cache.SessionExpired += Cache_SessionExpired;
        }

        private void Cache_SessionExpired(Session session)
        {
            lock (_lock)
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

        public void Message(Message clientMessage)
        {
            lock (_lock)
            {
                var session = GetSession();
                if (session != null)
                {
                    var destinationConnectionID = _connections.FirstOrDefault(x => x.Session.UserID == clientMessage.DestinationUserID)?.ConnectionID;
                    if (destinationConnectionID != null)
                    {
                        var msg = new Message()
                        {
                            DestinationUserID = clientMessage.DestinationUserID,
                            SenderUserID = session.UserID,
                            Text = clientMessage.Text
                        };
                        Clients.Client(destinationConnectionID).Message(msg);
                    }
                }
            }
        }

        public override Task OnConnected()
        {
            lock (_lock)
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
            lock (_lock)
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
            List<string> users = _connections.Select(x => x.Session.UserID).ToList();
            var msg = new ClientListBroadcast() {Clients = users};
            Clients.All.ClientList(msg);
        }
    }
}
