using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegistrarWebApi;

namespace Registrar
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new RegistrarHttpServer("http://localhost:9000");
            while (true)
            {
                server.Start();
                Console.ReadKey();
                server.Stop();
                Console.ReadKey();
            }
        }
    }
}
