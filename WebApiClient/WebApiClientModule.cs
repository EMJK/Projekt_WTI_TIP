using Castle.DynamicProxy;
using WebApiClient.Interfaces;

namespace WebApiClient
{
    public class WebApiClientModule
    {
        private readonly ProxyGenerator _proxyGenerator;
        private readonly IInterceptor _interceptor;

        public WebApiClientModule(string baseUrl)
        {
            _proxyGenerator = new ProxyGenerator();
            _interceptor = new RequestInterceptor(baseUrl);
        }

        public IAccount Account => _proxyGenerator.CreateInterfaceProxyWithoutTarget<IAccount>(_interceptor);
    }
}
