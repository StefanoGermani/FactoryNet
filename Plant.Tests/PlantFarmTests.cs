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
            Assert.IsInstanceOf<BasePlant>(plant);
        }

        [Test]
        public void Should_Cultivate_PersisterPlant()
        {
            var persister = MockRepository.GenerateMock<IPersisterSeed>();
            var plant = PlantFarm.CultivateWithBlueprintsFromAssemblyOf<TestBlueprint>(persister);
            Assert.IsInstanceOf<PersisterPlant>(plant);
        }

        [Test]
        public void Should_Load_Blueprints_From_Assembly()
        {
            var plant = PlantFarm.CultivateWithBlueprintsFromAssemblyOf<TestBlueprint>();
            Assert.AreEqual("Elaine", plant.Create<Person>().MiddleName);
        }
    }
}
