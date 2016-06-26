using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Castle.DynamicProxy;
using ChatClient.Client;
using ChatClient.Server;
using Microsoft.AspNet.SignalR.Client;
using Common;

namespace ChatClient
{
    public class ChatClientModule : IDisposable
    {
        private string _conID;
        private readonly HubConnection _connection;
        public IHubProxy<IServerMethods, IClientMethods> Hub { get; }



        public ChatClientModule(string baseUrl, string userID, string sessionID)
        {
            var queryString = new Dictionary<string, string>
            {
                [nameof(userID)] = userID,
                [nameof(sessionID)] = sessionID
            };

            _connection = new HubConnection(baseUrl, queryString);
            Hub = _connection.CreateHubProxy<IServerMethods, IClientMethods>("ChatHub");
            _connection.StateChanged += change =>
            {
                var newID = _connection.ConnectionId;
                if (!newID.IsNullOrEmpty() && _conID != newID)
                {
                    _conID = newID;
                }
            };

            _connection.Start().Wait();
        }

        

        private Type GetParamType(MethodInfo method)
        {
            var targetParams = method.GetParameters();
            if (targetParams.Length != 1) throw new InvalidOperationException();
            return targetParams[0].ParameterType;
        }

        public void Dispose()
        {
            Hub.Dispose();
            _connection.Dispose();
        }
    }
}
