using System;
using System.Configuration;
using Common;
using Julas.Utils;
using onvif.services;

namespace Server
{
    class AppConfig : IConfig
    {
        public string DatabaseConnectionString { get; }
        public string LocalIP { get; }
        public int LocalSipPort { get; }
        public Tuple<int, int> LocalRtpPortRange { get; }
        public int LocalChatPort { get; }
        public int LocalWebApiPort { get; }

        public AppConfig()
        {
            DatabaseConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            LocalIP = ConfigurationManager.AppSettings["LocalIP"];
            LocalSipPort = ConfigurationManager.AppSettings["LocalSipPort"].ConvertTo<int>();
            LocalRtpPortRange = Tuple.Create(
                ConfigurationManager.AppSettings["LocalRtpPortRange"].Split('-')[0].ConvertTo<int>(),
                ConfigurationManager.AppSettings["LocalRtpPortRange"].Split('-')[1].ConvertTo<int>());
            LocalChatPort = ConfigurationManager.AppSettings["LocalChatPort"].ConvertTo<int>();
            LocalWebApiPort = ConfigurationManager.AppSettings["LocalWebApiPort"].ConvertTo<int>();
        }
    }
}
