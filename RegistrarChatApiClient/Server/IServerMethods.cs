using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrarChatApiClient
{
    public interface IServerMethods
    {
        void SendMessage(SendMessageParam param);
    }
}
