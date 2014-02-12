using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using PlantFarm.Core.Dictionaries;
using PlantFarm.Core.Helpers;

namespace PlantFarm.Core
{
    internal class PlantFarmModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IConstructorHelper>().To<ConstructorHelper>();
            Bind<IBluePrintKeyHelper>().To<BluePrintKeyHelper>();

            Bind<IConstructorDictionary>().To<ConstructorDictionary>();
            Bind<ISequenceDictionary>().To<SequenceDictionary>();
            Bind<IPropertyDictionary>().To<PropertyDictionary>();
            Bind<ICreatedBlueprintsDictionary>().To<CreatedBlueprintsDictionary>();
            Bind<IPostCreationActionDictionary>().To<PostCreationActionDictionary>();

        }
    }
}
