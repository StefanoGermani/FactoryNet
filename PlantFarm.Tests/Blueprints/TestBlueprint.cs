using Plant.Tests.TestModels;
using PlantFarm.Core;

namespace PlantFarm.Tests.Blueprints
{
    class TestBlueprint : IBlueprint
    {
        public void SetupPlant(IPlant p)
        {
            p.Define(() => new House());
        }
    }
}
