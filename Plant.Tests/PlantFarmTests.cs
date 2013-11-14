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
            IPlant plant = PlantFarm.CultivateWithBlueprintsFromAssemblyOf<TestBlueprint>();
            Assert.AreEqual("Plant.Core.Impl.BasePlant", plant.GetType().ToString());
        }

        [Test]
        public void Should_Cultivate_PersisterPlant()
        {
            var persister = MockRepository.GenerateMock<IPersisterSeed>();
            IPlant plant = PlantFarm.Cultivate(persister);
            Assert.AreEqual("Plant.Core.Impl.PersisterPlant", plant.GetType().ToString());
        }

        [Test]
        public void Should_Load_Blueprints_From_Assembly()
        {
            IPlant plant = PlantFarm.CultivateWithBlueprintsFromAssemblyOf<TestBlueprint>();
            Assert.AreEqual("Plant.Core.Impl.BasePlant", plant.GetType().ToString());
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