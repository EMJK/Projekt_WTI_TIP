using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace SignalRClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var hubConnection = new HubConnection("http://localhost:9923/");
            var proxy = hubConnection.CreateHubProxy("chat");
            proxy.On<string>("message", s => Console.WriteLine($"Echo: {s}"));
            hubConnection.Start().Wait();
        }
    }
}
