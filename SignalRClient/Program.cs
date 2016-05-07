using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            Application.Run(new MainForm());
            //var webApiClient = new WebApiClient("http://localhost:9922/");
            //Console.Write("What is your name? ");
            //var name = Console.ReadLine();
            //var response2 = webApiClient.Account.Login(new LoginRequest() { UserID = name });
            //Console.WriteLine($"Logged in as `{name}`. SessionID: `{response2.SessionID}`");
            //var chatApiClient = new ChatApiClient("http://localhost:9923/", name, response2.SessionID);
            //SubscribeToChatEvents(chatApiClient);
            
            //Console.WriteLine($"Connected to SignalR server");

            //while (true)
            //{
            //    Console.WriteLine("Press Enter to log out");
            //    Console.ReadLine();
            //    webApiClient.Account.Logout(new LogoutRequest() { SessionID = response2.SessionID });
            //    Console.WriteLine("Logged out");
            //    Console.ReadLine();
            //}

            //while (true)
            //{
            //    Console.Write("Who do you want to send a message to? ");
            //    var other = Console.ReadLine();
            //    Console.Write("Enter the message: ");
            //    var str = Console.ReadLine();
            //    chatApiClient.Server.SendMessage(new SendMessageParam()
            //    {
            //        DestinationUserID = other,
            //        Message = str
            //    });
            //    Thread.Sleep(1000);
            //    webApiClient.Account.Logout(new LogoutRequest() {SessionID = response2.SessionID});
            //}
        }

        private static void SubscribeToChatEvents(ChatApiClient client)
        {
            client.Hub.SubscribeOn<MessageParam>(hub => hub.Message, msg =>
            {
                Console.WriteLine($"Message from {msg.SenderUserID}: {msg.Message}");
            });
            client.Hub.SubscribeOn<ClientListParam>(hub => hub.ClientList, list =>
            {
                var clients = list.Clients.Aggregate((x, y) => x + ", " + y);
                Console.WriteLine($"Clients: {clients}");
            });
        }
    }
}
