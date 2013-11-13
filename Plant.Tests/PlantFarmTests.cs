using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Plant.Core;
using Plant.Tests.TestBlueprints;
using Plant.Tests.TestModels;
using Rhino.Mocks;

namespace Plant.Tests
{
    [TestFixture]
    public class PlantFarmTests
    {
        [Test]
        public void Should_Cultivate_BasePlant()
        {
            var plant = PlantFarm.CultivateWithBlueprintsFromAssemblyOf<TestBlueprint>();
            Assert.AreEqual("Plant.Core.Impl.BasePlant", plant.GetType().ToString());
        }

        [Test]
        public void Should_Cultivate_PersisterPlant()
        {
            var persister = MockRepository.GenerateMock<IPersisterSeed>();
            var plant = PlantFarm.Cultivate(persister);
            Assert.AreEqual("Plant.Core.Impl.PersisterPlant", plant.GetType().ToString());
        }

        [Test]
        public void Should_Load_Blueprints_From_Assembly()
        {
            var plant = PlantFarm.CultivateWithBlueprintsFromAssemblyOf<TestBlueprint>();
            Assert.AreEqual("Plant.Core.Impl.BasePlant", plant.GetType().ToString());
            Assert.AreEqual("Elaine", plant.Create<Person>().MiddleName);
        }
    }

    namespace TestBlueprints
    {
        class TestBlueprint : IBlueprint
        {
            public void SetupPlant(IPlant plant)
            {
                plant.Define(() => new Person
                {
                    MiddleName = "Elaine"
                });
            }
        }
    }
}
