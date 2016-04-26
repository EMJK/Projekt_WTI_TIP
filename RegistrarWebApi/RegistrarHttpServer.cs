using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nancy;
using Nancy.Helpers;
using Nancy.Hosting.Self;
using Nancy.Owin;
using Ninject;

namespace RegistrarWebApi
{
    public class RegistrarHttpServer : IDisposable
    {
        private readonly IKernel _kernel;
        private NancyHost _nancyHost;

        public RegistrarHttpServer(string webApiUri, IKernel kernel)
        {
            _kernel = kernel;
            _kernel.Load<Nancy.Bootstrappers.Ninject.FactoryModule>();
            Start(webApiUri);
        }

        private void Start(string webApiUri)
        {
            _nancyHost = new NancyHost(new Uri(webApiUri), new NinjectBootstrapper(_kernel));
            _nancyHost.Start();
            Console.WriteLine($"HTTP server started at {webApiUri}");
        }

        private void Stop()
        {
            _nancyHost.Stop();
            _nancyHost.Dispose();
            _nancyHost = null;
            Console.WriteLine("HTTP server stopped");
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
