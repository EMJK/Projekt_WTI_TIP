using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;

namespace RegistrarChatApi
{
    public class RegistrarSignalRServer : IDisposable
    {
        private IDisposable _webApp;
        public RegistrarSignalRServer(string chatApiUri)
        {
            _webApp = WebApp.Start<Startup>(chatApiUri);
            Console.WriteLine($"Chat server started at {chatApiUri}");
        }

        public void Dispose()
        {
            _webApp.Dispose();
            _webApp = null;
            Console.WriteLine("Chat server stopped");
        }
        class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                app.UseCors(CorsOptions.AllowAll);
                app.MapSignalR();
            }
        }
    }
}
