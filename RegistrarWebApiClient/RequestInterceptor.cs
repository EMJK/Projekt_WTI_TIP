using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Newtonsoft.Json;

namespace RegistrarWebApiClient
{
    class RequestInterceptor : IInterceptor
    {
        private string _baseUrl;

        public RequestInterceptor(string _baseUrl)
        {
            this._baseUrl = _baseUrl;
        }

        public void Intercept(IInvocation invocation)
        {
            var moduleName = invocation.Method.DeclaringType.Name;
            if (moduleName.Take(2).All(Char.IsUpper))
            {
                moduleName = moduleName.Substring(1);
            }
            var methodName = invocation.Method.Name;
            var methodArg = JsonConvert.SerializeObject(invocation.Arguments[0]);
            var url = $"{_baseUrl.TrimEnd('/')}/{moduleName}/{methodName}/{methodArg}";
            var request = WebRequest.CreateHttp(url);
            request.Timeout = 3000000;
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var responseType = typeof (Response<>).MakeGenericType(new[] {invocation.Method.ReturnType});
            var responseObj = JsonConvert.DeserializeObject(responseString, responseType);
            var properties = responseType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            var responseCode = (int) properties.First(p => p.Name == "ResponseCode").GetValue(responseObj);
            if (responseCode / 100 != 2)
            {
                var responseMessage = (string) properties.First(p => p.Name == "ResponseMessage").GetValue(responseObj);
                throw new WebApiException(responseCode, responseMessage);
            }
            var body = properties.First(p => p.Name == "Body").GetValue(responseObj);
            invocation.ReturnValue = body;
        }
    }
}
