using CadeODinheiro.Web.Infrastructure.Provider.Abstract;
using CadeODinheiro.Web.Infrastructure.Provider.Concrete;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CadeODinheiro.Web.App_Start;
using CadeODinheiro.Core.DI;
using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.Business.Concrete;

namespace CadeODinheiro.Web.Infrastructure.DependencyResolver
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        public IKernel kernel { get; private set; }

        public NinjectDependencyResolver()
        {
            kernel = new StandardKernel(new NinjectModuleCustom());
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings(){
            kernel.Bind<IAuthProvider>().To<AuthProvider>();
            //kernel.Bind<IParameterBusiness>().To<ParameterBusiness>();
        }
    }
}