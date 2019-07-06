using Ninject.Modules;
using Ninject.Extensions.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.DI
{
    public class NinjectModuleCustom : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(c => c.FromThisAssembly()
                .SelectAllClasses()
                .BindDefaultInterfaces());
        }
    }
}
