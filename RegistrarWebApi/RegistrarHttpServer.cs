using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Host.HttpListener;
using Microsoft.Owin.Hosting;
using Nancy.Helpers;

namespace RegistrarWebApi
{
    public class RegistrarHttpServer
    {
        static RegistrarHttpServer()
        {
            OwinServerFactory.Initialize(new Dictionary<string, object>());
        }    

        private readonly object _syncObj = new object();
        private Task _task;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly string _baseAddress;

        public RegistrarHttpServer(string baseAddress)
        {
            _baseAddress = baseAddress;
        }
        public void Start()
        {
            lock (_syncObj)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                var token = _cancellationTokenSource.Token;
                _task = Task.Factory.StartNew(
                    () => ServerMethod(token), 
                    token, 
                    TaskCreationOptions.LongRunning, 
                    TaskScheduler.Default);
            }
        }

        public void Stop()
        {
            lock (_syncObj)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                _task.Wait();
                _task.Dispose();
                _task = null;
            }
        }

        private void ServerMethod(CancellationToken token)
        {
            using (WebApp.Start<Startup>(_baseAddress))
            {
                Console.WriteLine($"HTTP server at {_baseAddress} started");
                token.WaitHandle.WaitOne();
                Console.WriteLine($"HTTP server at {_baseAddress} stopped");
            }
        }
    }
}
