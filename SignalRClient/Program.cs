using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using RegistrarCommon.Chat;

namespace SignalRClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var queryStrig = new Dictionary<string, string>();
            queryStrig["UserName"] = "julkwiec";
            queryStrig["SessionKey"] = "123";
            var hubConnection = new HubConnection("http://localhost:9923/", queryStrig);
            var proxy = hubConnection.CreateHubProxy("ChatHub");
            proxy.On<Message>("Message", s => Console.WriteLine($"Received from {s.SenderUserID} to {s.DestinationUserID}: {s.Text}"));
            hubConnection.Start().Wait();
            proxy.Invoke("Message", new Message()
            {
                DestinationUserID = "julkwiec",
                SenderUserID = "julkwiec",
                Text = "Hola amigo!"

            }).Wait();
            Console.ReadLine();
        }
    }
}
