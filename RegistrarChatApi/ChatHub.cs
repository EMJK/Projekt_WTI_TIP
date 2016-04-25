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

        public void Message(IncomingMessage incomingMessage)
        {
            var entry = _cache.GetSession(incomingMessage.UserID);
            if (entry.SessionID == incomingMessage.SessionID)
            {
                string destinationConnectionID;
                lock (_lock)
                {
                    destinationConnectionID =
                        _connections.FirstOrDefault(x => x.UserID == incomingMessage.DestinationUserID)?.ConnectionID;
                }
                if (destinationConnectionID != null)
                {
                    var msg = new OutgoingMessage()
                    {
                        DestinationUserID = incomingMessage.DestinationUserID, 
                        SenderUserID = incomingMessage.UserID, 
                        Text = incomingMessage.Text
                    };
                    Clients.Client(destinationConnectionID).Message(msg);
                }
            }
        }

        public void Register(RegisterRequest register)
        {
            var entry = _cache.GetSession(register.UserID);
            if (entry?.SessionID == register.SessionID)
            {
                _cache.CreateSession(register.UserID);
                lock (_lock)
                {
                    _connections.Add(new Connection() {ConnectionID = Context.ConnectionId, UserID = register.UserID});
                }
                Clients.Caller.RegisterResponse(new RegisterResponse() {Success = true});
                SendClientList();
            }
            else
            {
                Clients.Caller.RegisterResponse(new RegisterResponse() { Success = false, StatusMessage = "Invalid session" });
            }
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
