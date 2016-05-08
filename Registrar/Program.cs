using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Ninject;
using Owin;
using RegistrarChatApi;
using RegistrarCommon;
using RegistrarSipApi;
using RegistrarWebApi;

namespace Registrar
{
    class Program
    {
        private static readonly SessionCache Cache = new SessionCache(TimeSpan.FromMinutes(5));

        static void Main(string[] args)
        {
            string host = "localhost";
            int webApiPort = 9922;
            int sipPort = 5060;
            int chatApiPort = 9923;

            var webApiUri = $"http://{host}:{webApiPort}/";
            var chatApiUri = $"http://{host}:{chatApiPort}/";

            var sipserver = new RegistrarSipServer(5060, "10.0.0.4");

            var kernel = GetNinjectKernel();

            sipserver.Start();
            using (new RegistrarHttpServer(webApiUri, kernel))
            using (new RegistrarSignalRServer(chatApiUri, kernel))
            //using (new RegistrarSipServer(host, sipPort, kernel))
            {
                Console.ReadLine();
            }
        }

        private static IKernel GetNinjectKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<ISessionCache>().ToMethod(c => Cache);
            return kernel;
        }
    }
}
