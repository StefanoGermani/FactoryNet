using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Plant.Core.Impl;

namespace Plant.Core
{
    public class PlantFarm
    {
        public static IPlant CultivateWithBlueprintsFromAssemblyOf<T>()
        {
            var plant = new BasePlant();

            return LoadAssembly<T>(plant);
        }

        public static IPlant CultivateWithBlueprintsFromAssemblyOf<T>(IPersisterSeed persisterSeed)
        {
            var plant = new PersisterPlant(persisterSeed);

            return LoadAssembly<T>(plant);
        }

        private static IPlant LoadAssembly<T>(IPlant plant)
        {
            Assembly assembly = typeof (T).Assembly;
            IEnumerable<Type> blueprintTypes = assembly.GetTypes().Where(t => typeof (IBlueprint).IsAssignableFrom(t));
            blueprintTypes.ToList().ForEach(blueprintType =>
                {
                    var blueprint = (IBlueprint) Activator.CreateInstance(blueprintType);
                    blueprint.SetupPlant(plant);
                });
            return plant;
        }

        public static IPlant Cultivate()
        {
            return new BasePlant();
        }

        public static IPlant Cultivate(IPersisterSeed persisterSeed)
        {
            return new PersisterPlant(persisterSeed);
        }
    }
}