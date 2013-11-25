using NUnit.Framework;
using Plant.Tests.TestBlueprints;
using Plant.Tests.TestModels;
using PlantFarm.Core;
using Rhino.Mocks;

namespace Plant.Tests
{
    [TestFixture]
    public class PlantFarmTests
    {
        [Test]
        public void Should_Cultivate_BasePlant()
        {
            IPlant plant = Farm.CultivateWithBlueprintsFromAssemblyOf<TestBlueprint>();
            Assert.AreEqual("PlantFarm.Core.Impl.BasePlant", plant.GetType().ToString());
        }

        [Test]
        public void Should_Cultivate_PersisterPlant()
        {
            var persister = MockRepository.GenerateMock<IPersisterSeed>();
            IPlant plant = Farm.Cultivate(persister);
            Assert.AreEqual("PlantFarm.Core.Impl.PersisterPlant", plant.GetType().ToString());
        }

        [Test]
        public void Should_Load_Blueprints_From_Assembly()
        {
            IPlant plant = Farm.CultivateWithBlueprintsFromAssemblyOf<TestBlueprint>();
            Assert.AreEqual("PlantFarm.Core.Impl.BasePlant", plant.GetType().ToString());
            Assert.AreEqual("Elaine", plant.Create<Person>().MiddleName);
        }
    }

    namespace TestBlueprints
    {
        internal class TestBlueprint : IBlueprint
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