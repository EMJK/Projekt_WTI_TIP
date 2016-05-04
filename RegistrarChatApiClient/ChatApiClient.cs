using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.AspNet.SignalR.Client;
using RegistrarCommon;

namespace RegistrarChatApiClient
{
    public class ChatApiClient : IDisposable
    {
        public IServerMethods Server { get; }
        private readonly Action _dispose;

        public ChatApiClient(string baseUrl, string userID, string sessionID, IClientMethods client)
        {
            var queryString = new Dictionary<string, string>
            {
                [nameof(userID)] = userID,
                [nameof(sessionID)] = sessionID
            };

            var connection = new HubConnection(baseUrl, queryString);
            var proxy = connection.CreateHubProxy("ChatHub");
            Server = new ProxyGenerator().CreateInterfaceProxyWithoutTarget<IServerMethods>(
                new SignalRInterceptor(proxy));
            var binding = BindClient(proxy, client);
            _dispose = () =>
            {
                binding.Dispose();
                connection.Dispose();
            };

            connection.Start().Wait();
        }

        private IDisposable BindClient(IHubProxy proxy, IClientMethods client)
        {
            var methods = typeof (IClientMethods)
                .GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            var genericHandler = typeof (HubProxyExtensions)
                .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Single(m => m.Name == "On"
                            && m.IsGenericMethodDefinition
                            && m.GetGenericArguments().Length == 1
                            && m.GetParameters().Length == 3);
            var disposables = new List<IDisposable>();
            foreach (var method in methods)
            {
                var targetMethodParamType = GetParamType(method);
                var targetMethodDelegate = GetTargetMethodDelegate(method, client);
                var binderMethod = genericHandler.MakeGenericMethod(targetMethodParamType);
                var disposable = binderMethod.Invoke(null, new[] { proxy, method.Name, targetMethodDelegate });
                disposables.Add((IDisposable)disposable);
            }
            disposables.Reverse();
            return Disposable.Create(() =>
            {
                foreach (var disposable in disposables)
                {
                    disposable.Dispose();
                }
            });
        }

        private Type GetParamType(MethodInfo method)
        {
            var targetParams = method.GetParameters();
            if (targetParams.Length != 1) throw new InvalidOperationException();
            return targetParams[0].ParameterType;
        }

        private object GetTargetMethodDelegate(MethodInfo method, IClientMethods client)
        {
            var paramType = method.GetParameters()[0].ParameterType;
            return Delegate.CreateDelegate(
                typeof (Action<>).MakeGenericType(new[] { paramType }),
                client,
                method);
        }

        public void Dispose()
        {
            _dispose?.Invoke();
        }
    }

    public class SignalRInterceptor : IInterceptor
    {
        private readonly IHubProxy _proxy;
        public SignalRInterceptor(IHubProxy proxy)
        {
            _proxy = proxy;
        }

        public void Intercept(IInvocation invocation)
        {
            _proxy.Invoke(invocation.Method.Name, invocation.Arguments).Wait();
        }
    }
}
