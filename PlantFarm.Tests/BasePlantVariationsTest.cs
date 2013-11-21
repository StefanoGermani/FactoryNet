using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Plant.Core;
using Plant.Tests.TestModels;

namespace Plant.Tests
{
    [TestFixture]
    public class BasePlantVariationsTest
    {
        [SetUp]
        public void SetUp()
        {
            _plant = PlantFarm.Cultivate();
        }

        private IPlant _plant;


        [Test]
        public void Should_Create_Variation_Of_Specified_Type()
        {
            _plant.Define(() => new Person { FirstName = "" });
            _plant.Define("My", () => new Person { FirstName = "My" });
            _plant.Define("Her", () => new Person { FirstName = "Her" });

            Assert.IsInstanceOf(typeof(Person), _plant.Create<Person>());
            Assert.IsInstanceOf(typeof(Person), _plant.Create<Person>("My"));
            Assert.IsInstanceOf(typeof(Person), _plant.Create<Person>("Her"));
        }

        [Test]
        public void Should_Create_Variation_Of_Specified_Type_With_Correct_Data()
        {
            _plant.Define(() => new Person { FirstName = "" });
            _plant.Define("My", () => new Person { FirstName = "My" });

            var person = _plant.Create<Person>("My");
            Assert.AreEqual("My", person.FirstName);
        }

        [Test]
        public void Should_Create_Variation_With_Extension()
        {
            _plant.Define(() => new House { Color = "blue" }, OnPropertyPopulation);
            _plant.Define("My", () => new House { Color = "My" }, OnPropertyPopulationVariation);

            Assert.AreEqual(_plant.Create<House>().Persons.First().FirstName, "Pablo");
            Assert.AreEqual(_plant.Create<House>("My").Persons.First().FirstName, "Pedro");
        }

        private static void OnPropertyPopulation(House h)
        {
            h.Persons.Add(new Person { FirstName = "Pablo" });
        }

        private static void OnPropertyPopulationVariation(House h)
        {
            h.Persons.Clear();
            h.Persons.Add(new Person { FirstName = "Pedro" });
        }


    }
}
