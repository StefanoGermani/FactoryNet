using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            var assembly = typeof (T).Assembly;
            var blueprintTypes = assembly.GetTypes().Where(t => typeof (IBlueprint).IsAssignableFrom(t));
            blueprintTypes.ToList().ForEach(blueprintType =>
                {
                    var blueprint = (IBlueprint) Activator.CreateInstance(blueprintType);
                    blueprint.SetupPlant(plant);
                });
            return plant;
        }

        
    }
}
