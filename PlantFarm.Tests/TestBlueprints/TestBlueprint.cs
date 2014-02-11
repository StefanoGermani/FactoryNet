using PlantFarm.Core;
using PlantFarm.Tests.TestModels;

namespace PlantFarm.Tests.TestBlueprints
{
    class TestBlueprint : IBlueprint
    {
        public void SetupPlant(IPlant p)
        {
            p.Define(() => new House());
        }
    }
}
