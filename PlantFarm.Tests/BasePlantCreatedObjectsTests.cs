using System;
using System.Linq;
using NUnit.Framework;
using Plant.Core;
using Plant.Core.Exceptions;
using Plant.Core.Impl;
using Plant.Tests.TestModels;
using Rhino.Mocks;

namespace Plant.Tests
{
    [TestFixture]
    public class BasePlantCreatedObjectsTests
    {
        [SetUp]
        public void SetUp()
        {
            _plant = PlantFarm.Cultivate();
        }

        private IPlant _plant;

        [Test]
        public void Should_Return_A_List_Of_Created_Objects()
        {
            _plant.Define(() => new House { Color = "blue", SquareFoot = 50 });
            var house = _plant.Create<House>();
            var redHouse = _plant.Create<House>(h => h.Color = "red");

            Assert.AreEqual(2, _plant.CreatedObjects.Count);
            Assert.AreEqual(house, _plant.CreatedObjects[0]);
            Assert.AreEqual(redHouse, _plant.CreatedObjects[1]);
        }

        [Test]
        public void Should_Clear_List_Of_Created_Objects()
        {
            _plant.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _plant.Create<House>();
            _plant.Create<House>(h => h.Color = "red");

            _plant.ClearCreatedObjects();

            Assert.AreEqual(0, _plant.CreatedObjects.Count);
        }

        [Test]
        public void Is_Event_Deleted_Called()
        {
            var dummy = MockRepository.GenerateStub<IDummy>();

            dummy.Stub(d => d.Test());

            _plant.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _plant.Create<House>();
            _plant.Create<House>();

            _plant.ObjectDeleted += (sender, e) => dummy.Test();

            _plant.ClearCreatedObjects();

            dummy.AssertWasCalled(x=> x.Test(), options => options.Repeat.Twice());
        }
    }
}