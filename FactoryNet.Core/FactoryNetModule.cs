using FactoryNet.Core.Dictionaries;
using FactoryNet.Core.Helpers;
using Ninject.Modules;

namespace FactoryNet.Core
{
    internal class FactoryNetModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IConstructorHelper>().To<ConstructorHelper>();
            Bind<IBluePrintKeyHelper>().To<BluePrintKeyHelper>();

            Bind<IConstructorList>().To<ConstructorList>();
            Bind<ISequenceDictionary>().To<SequenceDictionary>();
            Bind<IPropertyDictionary>().To<PropertyDictionary>();
            Bind<ICreatedBlueprintsDictionary>().To<CreatedBlueprintsDictionary>();
            Bind<IPostCreationActionDictionary>().To<PostCreationActionDictionary>();

        }
    }
}
