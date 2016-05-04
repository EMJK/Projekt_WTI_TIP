using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using RegistrarChatApiClient;
using RegistrarWebApiClient;
using RegistrarWebApiClient.Models.Account;

namespace SignalRClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var webApiClient = new WebApiClient("http://localhost:9922/");
            var response2 = webApiClient.Account.Login(new LoginRequest() { UserID = "HewiMetal" });
            Console.WriteLine($"Logged in as `HewiMetal`. SessionID: `{response2.SessionID}`");
            var chatApiClient = new ChatApiClient("http://localhost:9923/", "HewiMetal", response2.SessionID, new ClientMethods());
            Console.WriteLine($"Connected to SignalR server");

            while (true)
            {
                var str = Console.ReadLine();
                chatApiClient.Server.SendMessage(new SendMessageParam()
                {
                    DestinationUserID = "HewiMetal",
                    Message = str
                });
            }
        }

        class ClientMethods : IClientMethods
        {
            public void Message(MessageParam param)
            {
                Console.WriteLine($"[Message] From: {param.SenderUserID}; Message: {param.Message}");
            }

            public void ClientList(ClientListParam param)
            {
                var clients = param.Clients.Aggregate((x, y) => x + ", " + y);
                Console.WriteLine($"[ClientList] Clients: {clients}");
            }
        }
    }
}
