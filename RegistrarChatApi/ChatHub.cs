using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RegistrarCommon;

namespace RegistrarChatApi
{
    [HubName("chat")]
    public class ChatHub : Hub
    {
        public ChatHub(ISessionCache cache)
        {
            
        }

        public void Message(string name, string message)
        {
            //Clients.
            //Context.Request
        }
    }
}
