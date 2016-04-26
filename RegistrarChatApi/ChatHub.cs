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
        private readonly HashSet<Connection> _connections;
        private readonly object _lock = new object();

        public ChatHub(ISessionCache cache)
        {
            _cache = cache;
            _connections = new HashSet<Connection>();
            cache.SessionExpired += Cache_SessionExpired;
        }

        private void Cache_SessionExpired(Session session)
        {
            lock (_lock)
            {
                _connections.RemoveWhere(x => x.UserID == session.UserID);
            }
            SendClientList();
        }

        private Session GetSession()
        {
            lock (_lock)
            {
                var con = _connections.FirstOrDefault(x => x.ConnectionID == Context.ConnectionId);
                if (con == null) return null;
                return _cache.GetSession(con.UserID);
            }
        }

        public void Message(Message clientMessage)
        {
            var session = GetSession();
            if (session != null)
            {
                string destinationConnectionID;
                lock (_lock)
                {
                    destinationConnectionID =
                        _connections.FirstOrDefault(x => x.UserID == clientMessage.DestinationUserID)?.ConnectionID;
                }
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

        public override Task OnConnected()
        {
            var user = Context.QueryString["UserName"];
            var key = Context.QueryString["SessionKey"];
            var session = _cache.GetSession(user);
            if (session != null && session.SessionID == key)
            {
                lock (_lock)
                {
                    _connections.Add(new Connection()
                    {
                        ConnectionID = Context.ConnectionId,
                        UserID = user
                    });
                }
            }
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            var user = Context.QueryString["UserName"];
            var key = Context.QueryString["SessionKey"];
            var session = _cache.GetSession(user);
            if (session != null && session.SessionID == key)
            {
                lock (_lock)
                {
                    _connections.Add(new Connection()
                    {
                        ConnectionID = Context.ConnectionId,
                        UserID = user
                    });
                }
            }
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            lock (_lock)
            {
                _connections.RemoveWhere(x => x.ConnectionID == Context.ConnectionId);
            }
            SendClientList();
            return base.OnDisconnected(stopCalled);
        }

        private void SendClientList()
        {
            List<string> users;
            lock (_lock)
            {
                users = _connections.Select(x => x.UserID).ToList();
            }
            var msg = new ClientListBroadcast() {Clients = users};
            Clients.All.ClientList(msg);
        }
    }
}
