using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FactoryNet.Core.Helpers
{
    public interface ILoaderHelper
    {
        IFactory LoadBlueprintsFromCurrentAssembly(Assembly callingAssembly);
        IFactory LoadBlueprintsFromAssembly(Assembly assembly);
        IFactory LoadBlueprintsFromAssemblies();
    }

    public class LoaderHelper : ILoaderHelper
    {
        private readonly IFactory _factory;

        public LoaderHelper(IFactory factory)
        {
            _factory = factory;
        }

        public IFactory LoadBlueprintsFromCurrentAssembly(Assembly callingAssembly)
        {
            var blueprintTypes = callingAssembly.GetTypes().Where(t => t.IsClass && typeof(IBlueprint).IsAssignableFrom(t));
            blueprintTypes.ToList().ForEach(blueprintType =>
            {
                var blueprint = (IBlueprint)Activator.CreateInstance(blueprintType);
                blueprint.SetupFactory(_factory);
            });

            return _factory;
        }

        public IFactory LoadBlueprintsFromAssembly(Assembly assembly)
        {
            var blueprintTypes = assembly.GetTypes().Where(t => t.IsClass && typeof(IBlueprint).IsAssignableFrom(t));
            blueprintTypes.ToList().ForEach(blueprintType =>
            {
                var blueprint = (IBlueprint)Activator.CreateInstance(blueprintType);
                blueprint.SetupFactory(_factory);
            });

            return _factory;
        }

        public IFactory LoadBlueprintsFromAssemblies()
        {
            AppDomain.CurrentDomain.GetAssemblies().ToList().ForEach(t => LoadBlueprintsFromAssembly(t));

            return _factory;
        }

    }
}
