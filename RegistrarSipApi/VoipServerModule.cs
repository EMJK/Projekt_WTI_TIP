using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Ninject;

namespace VoipServer
{
    public class VoipServerModule : IDisposable
    {
        private readonly IKernel _kernel;
        private readonly IConfig _config;
        private SipServer _sipServer;

        public VoipServerModule(IKernel kernel)
        {
            _kernel = kernel;
            _config = _kernel.Get<IConfig>();
            _sipServer = new SipServer(
                _config.LocalIP, 
                _config.LocalSipPort, 
                _config.LocalRtpPortRange.Item1, 
                _config.LocalRtpPortRange.Item2,
                _config.DatabaseConnectionString);
            _sipServer.Start();
            Console.WriteLine($"VoIP server started at {_config.LocalIP}:{_config.LocalSipPort} (UDP)");
        }

        public void Dispose()
        {
            _sipServer.Stop();
            Console.WriteLine("VoIP server stopped");
        }
    }
}
