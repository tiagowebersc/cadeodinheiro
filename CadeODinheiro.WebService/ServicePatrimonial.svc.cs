using Ninject;
using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.Business.Concrete;
using CadeODinheiro.Core.DI;
using CadeODinheiro.Core.Entity;
using CadeODinheiro.Core.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Transactions;

namespace CadeODinheiro.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IServiceCadeODinheiro
    {
        protected IKernel kernel;
        public Service1()
        {
            kernel = new StandardKernel(new NinjectModuleCustom());
            kernel.Inject(this);
        }

        public string retornoTeste()
        {
            return "OK";
        }
    }
}
