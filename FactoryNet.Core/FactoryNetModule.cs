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

            Bind<IConstructors>().To<Constructors>();
            Bind<ISequences>().To<Sequences>();
            Bind<IProperties>().To<Properties>();
            Bind<ICreatedBlueprints>().To<CreatedBlueprints>();
            Bind<IPostCreationActions>().To<PostCreationActions>();
        }
    }
}
