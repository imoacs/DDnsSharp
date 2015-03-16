using DDnsSharp.Core.Services;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDnsSharp.Core
{
    public class DDnsSharpIoC
    {
        static DDnsSharpIoC()
        {
            kernel.Load(new DefaultNinjectModule());
        }

        private static IKernel kernel = new StandardKernel();

        public static IKernel Current
        {
            get { return kernel; }
        }

        public class DefaultNinjectModule : NinjectModule
        {
            public override void Load()
            {
                Bind<IServiceHelper>().To<ServiceHelper>();
            }
        }
    }
}
