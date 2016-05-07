using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Julas.Utils.Extensions;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Ninject;
using Owin;
using RegistrarChatApiClient;
using RegistrarCommon;

namespace RegistrarChatApi
{
    public class RegistrarSignalRServer : IDisposable
    {
        private readonly object _clientListLock = new object();
        internal static IKernel Kernel;
        private IDisposable _webApp;
        private readonly ISessionCache _sessionCache;
        private readonly IConnectionCache _connectionCache;

        public RegistrarSignalRServer(string chatApiUri, IKernel kernel)
        {
            Kernel = kernel;
            _sessionCache = Kernel.Get<ISessionCache>();
            _connectionCache = new ConnectionCache(_sessionCache);
            Kernel.Bind<IConnectionCache>().ToMethod(x => _connectionCache);
            Kernel.Bind<RegistrarSignalRServer>().ToMethod(x => this);
            _sessionCache.SessionClosed += SessionCacheOnSessionClosed;
            _webApp = WebApp.Start(chatApiUri, Configuration);
            Console.WriteLine($"Chat server started at {chatApiUri}");
        }

        private void SessionCacheOnSessionClosed(Session session)
        {
            _connectionCache.RemoveConnectionForSessionID(session.SessionID);
            SendClientList();
        }

        internal void SendClientList()
        {
            Task.Run(async () =>
            {
                await Task.Delay(750);

                var connections = _connectionCache.GetValidConnections();
                var ids = connections.Select(c => c.ConnectionID).ToList();
                var msg = new ClientListParam()
                {
                    Clients = connections.Select(c => c.UserID).ToList()
                };
                var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub, IClientMethods>();
                hub.Clients.Clients(ids).ClientList(msg);
            });
        }

        public void Dispose()
        {
            _webApp.Dispose();
            _webApp = null;
            _sessionCache.SessionClosed -= SessionCacheOnSessionClosed;
            Console.WriteLine("Chat server stopped");
        }

        private void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                map.RunSignalR();
            });
        }
    }
}
