using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Ninject;
using ChatClient;
using ChatClient.Client;
using ChatClient.Server;
using Common;

namespace ChatServer
{
    public class ChatHub : Hub<IClientMethods>, IServerMethods
    {
        private readonly IConnectionCache _connectionCache;
        private readonly ChatServerModule _serverModule;

        public ChatHub()
        {
            _connectionCache = ChatServerModule.Kernel.Get<IConnectionCache>();
            _serverModule = ChatServerModule.Kernel.Get<ChatServerModule>();
        }

        private Connection GetCurrentConnection()
        {
            return new Connection()
            {
                ConnectionID = Context.ConnectionId,
                SessionID = Context.QueryString.Get("SessionID")?.ToLower() ?? string.Empty,
                UserID = Context.QueryString.Get("UserID")?.ToLower() ?? string.Empty
            };
        }

        public override async Task OnConnected()
        {
            _connectionCache.Verify(GetCurrentConnection());
            await base.OnConnected();

            _serverModule.SendClientList();
        }

        public override async Task OnReconnected()
        {
            _connectionCache.Verify(GetCurrentConnection());
            await base.OnReconnected();
            _serverModule.SendClientList();
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            _connectionCache.RemoveConnectionForConnectionID(Context.ConnectionId);
            await base.OnDisconnected(stopCalled);
            _serverModule.SendClientList();
        }

        public void SendMessage(SendMessageParam param)
        {
            var dst = _connectionCache.GetConnectionIDForUser(param.DestinationUserID);
            var con = GetCurrentConnection();
            if (_connectionCache.Verify(con) && dst != null)
            {
                var msg = new MessageParam()
                {
                    Message = param.Message,
                    SenderUserID = con.UserID
                };
                Clients.Client(dst).Message(msg);
            }
        }
    }
}
