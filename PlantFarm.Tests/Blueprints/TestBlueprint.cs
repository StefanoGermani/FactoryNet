using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plant.Tests.TestModels;
using PlantFarm.Core;

namespace Plant.Tests.Blueprints
{
    class TestBlueprint : IBlueprint
    {
        public void SetupPlant(IPlant p)
        {
            p.Define(() => new House());
        }
    }
}
