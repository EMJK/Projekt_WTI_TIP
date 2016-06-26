using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoipClient
{
    public enum PhoneStatus
    {
        Unregistered,
        Registering,
        Registered,
        IncomingCall,
        Calling,
        InCall
    }

    public class PhoneState
    {
        public PhoneStatus Status { get; set; }
        public string OtherUserId { get; set; }
    }
}
