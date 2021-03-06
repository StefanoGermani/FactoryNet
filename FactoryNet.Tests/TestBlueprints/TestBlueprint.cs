﻿using FactoryNet.Core;
using FactoryNet.Tests.TestModels;

namespace FactoryNet.Tests.TestBlueprints
{
    class TestBlueprint : IBlueprint
    {
        public void SetupFactory(IFactory p)
        {
            p.Define(() => new House());
        }
    }
}
