using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plant.Core
{
    public class PlantFarm
    {
        public static IPlant WithBlueprintsFromAssemblyOf<T>()
        {
            var plant = new BasePlant();

            var assembly = typeof(T).Assembly;
            var blueprintTypes = assembly.GetTypes().Where(t => typeof(IBlueprint).IsAssignableFrom(t));
            blueprintTypes.ToList().ForEach(blueprintType =>
                {
                    var blueprint = (IBlueprint)Activator.CreateInstance(blueprintType);
                    blueprint.SetupPlant(plant);
                });
            return plant;

        }

        public static IPlant CreateDatabasePlant(IPersisterSeed persisterSeed)
        {
            return new PersisterPlant(persisterSeed);
        }
    }
}
