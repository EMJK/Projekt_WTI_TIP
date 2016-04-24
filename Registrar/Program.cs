using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Owin;
using RegistrarChatApi;
using RegistrarSipApi;
using RegistrarWebApi;

namespace Registrar
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = "localhost";
            int webApiPort = 9922;
            int sipPort = 5060;
            int chatApiPort = 9923;

            var webApiUri = $"http://{host}:{webApiPort}/";
            var chatApiUri = $"http://{host}:{chatApiPort}/";

            using (new RegistrarHttpServer(webApiUri))
            using (new RegistrarSignalRServer(chatApiUri))
            using (new RegistrarSipServer(host, sipPort))
            {
                Console.ReadLine();
            }
        }
    }
}
