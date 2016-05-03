using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using RegistrarCommon.Chat;
using RegistrarWebApiClient;
using RegistrarWebApiClient.Models.Account;

namespace SignalRClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new WebApiClient("http://localhost:9922/");
            var response = client.Account.Register(new RegisterAccountRequest() {UserName = "HewiMetal"});

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
