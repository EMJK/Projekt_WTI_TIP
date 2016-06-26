using System;

namespace Common
{
    public interface IConfig
    {
        string DatabaseConnectionString { get; }
        string LocalIP { get; }
        int LocalSipPort { get; }
        Tuple<int,int> LocalRtpPortRange { get; }
        int LocalChatPort { get; }
        int LocalWebApiPort { get; }
    }
}