﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Ninject;
using Owin;

namespace RegistrarChatApi
{
    public class RegistrarSignalRServer : IDisposable
    {
        internal static IKernel Kernel;
        private IDisposable _webApp;

        public RegistrarSignalRServer(string chatApiUri, IKernel kernel)
        {
            var connections = new List<Connection>();
            Kernel = kernel;
            Kernel.Bind<IList<Connection>>().ToMethod(x => connections);
            _webApp = WebApp.Start(chatApiUri, Configuration);
            Console.WriteLine($"Chat server started at {chatApiUri}");
        }

        public void Dispose()
        {
            _webApp.Dispose();
            _webApp = null;
            Console.WriteLine("Chat server stopped");
        }

        private void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.Map("/signalr", map =>
            {

                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration()
                {
                    EnableDetailedErrors = true,
                    Resolver = new NinjectDependencyResolver(Kernel)
                };
                map.RunSignalR(hubConfiguration);
            });
        }
    }
}
