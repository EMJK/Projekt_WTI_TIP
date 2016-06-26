using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Bootstrappers.Ninject;
using Ninject;
using Npgsql;

namespace WebApiServer
{
    class NinjectBootstrapper : NinjectNancyBootstrapper
    {
        private readonly IKernel _kernel;

        public NinjectBootstrapper(IKernel kernel)
        {
            _kernel = kernel;
        }

        protected override IKernel GetApplicationContainer()
        {
            return _kernel;
        }

        protected override void ConfigureRequestContainer(IKernel container, NancyContext context)
        {
            var connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            container.Bind<NpgsqlConnection>().ToConstant(connection).InSingletonScope();
        }
    }
}
