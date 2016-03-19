using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Owin;

namespace RegistrarWebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "SIP Registrar",
                "registrar/{controller}/{id}",
                new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        }
    }
}