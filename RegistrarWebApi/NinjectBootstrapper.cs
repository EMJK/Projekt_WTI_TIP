using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Bootstrappers.Ninject;
using Ninject;

namespace RegistrarWebApi
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
            //tutaj robimy dbconnection
        }
    }
}
