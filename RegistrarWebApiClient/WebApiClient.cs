using Castle.DynamicProxy;
using RegistrarWebApiClient.Interfaces;

namespace RegistrarWebApiClient
{
    public class WebApiClient
    {
        private readonly ProxyGenerator _proxyGenerator;
        private readonly IInterceptor _interceptor;

        public WebApiClient(string baseUrl)
        {
            _proxyGenerator = new ProxyGenerator();
            _interceptor = new RequestInterceptor(baseUrl);
        }

        public IAccount Account => _proxyGenerator.CreateInterfaceProxyWithoutTarget<IAccount>(_interceptor);
    }
}
