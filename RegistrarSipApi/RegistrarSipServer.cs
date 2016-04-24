using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrarSipApi
{
    public class RegistrarSipServer : IDisposable
    {
        public RegistrarSipServer(string host, int port)
        {
            Console.WriteLine($"SIP server started at {host}:{port}");
        }

        public void Dispose()
        {
            Console.WriteLine("SIP server stopped");
        }
    }
}
