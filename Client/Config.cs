using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Julas.Utils;

namespace Client
{
    class Config
    {
        public string ServerIP { get; }
        public int SipPort { get; }
        public int WebApiPort { get; }
        public int ChatPort { get; }

        public Config()
        {
            ServerIP = ConfigurationManager.AppSettings["ServerIP"];
            SipPort = ConfigurationManager.AppSettings["SipPort"].ConvertTo<int>();
            ChatPort = ConfigurationManager.AppSettings["ChatPort"].ConvertTo<int>();
            WebApiPort = ConfigurationManager.AppSettings["WebApiPort"].ConvertTo<int>();
        }
    }
}
