using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using PlantFarm.Core.Helpers;

namespace PlantFarm.Core
{
    internal class PlantFarmModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IConstructorHelper>().To<ConstructorHelper>();
            Bind<IBluePrintKeyHelper>().To<BluePrintKeyHelper>();
        }
    }
}
