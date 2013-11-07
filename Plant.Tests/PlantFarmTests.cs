using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Plant.Core;
using Plant.Tests.TestBlueprints;
using Plant.Tests.TestModels;

namespace Plant.Tests
{
    [TestFixture]
    public class PlantFarmTests
    {
        [Test]
        public void Should_Load_Blueprints_From_Assembly()
        {
            var plant = PlantFarm.WithBlueprintsFromAssemblyOf<TestBlueprint>();
            Assert.AreEqual("Elaine", plant.Create<Person>().MiddleName);
        }
    }
}
