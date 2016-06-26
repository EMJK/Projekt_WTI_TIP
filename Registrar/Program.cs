using System;
using Ninject;
using Ozeki.Network;
using ChatServer;
using Common;
using VoipServer;
using WebApiServer;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server...");

            var kernel = CreateNinjectKernel();
            
            using (new WebApiServerModule(kernel))
            using (new ChatServerModule(kernel))
            using (new VoipServerModule(kernel))
            {
                Console.WriteLine("All modules started. Press ENTER to exit...");
                Console.ReadLine();
            }
        }

        private static IKernel CreateNinjectKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<ISessionCache>().ToConstant(new SessionCache(TimeSpan.FromMinutes(5))).InSingletonScope();
            kernel.Bind<IConfig>().ToConstant(new AppConfig()).InSingletonScope();
            return kernel;
        }
    }
}
