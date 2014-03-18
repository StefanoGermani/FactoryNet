using FactoryNet.Core;
using FactoryNet.Tests.TestModels;
using NUnit.Framework;
using Rhino.Mocks;

namespace FactoryNet.Tests
{
    [TestFixture]
    public class BaseFactoryCreatedObjectsTests
    {
        [SetUp]
        public void SetUp()
        {
            _factory = new BaseFactory();
        }

        private IFactory _factory;

        [Test]
        public void Should_Return_A_List_Of_Created_Objects()
        {
            _factory.Define(() => new House { Color = "blue", SquareFoot = 50 });
            var house = _factory.Create<House>();
            var redHouse = _factory.Create<House>(h => h.Color = "red");

            Assert.AreEqual(2, _factory.CreatedObjects.Count);
            Assert.AreEqual(house, _factory.CreatedObjects[0]);
            Assert.AreEqual(redHouse, _factory.CreatedObjects[1]);
        }

        [Test]
        public void Should_Clear_List_Of_Created_Objects()
        {
            _factory.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _factory.Create<House>();
            _factory.Create<House>(h => h.Color = "red");

            _factory.ClearCreatedObjects();

            Assert.AreEqual(0, _factory.CreatedObjects.Count);
        }

        [Test]
        public void Is_Event_Deleted_Called()
        {
            var dummy = MockRepository.GenerateStub<IDummy>();

            dummy.Stub(d => d.Test());

            _factory.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _factory.Create<House>();
            _factory.Create<House>();

            _factory.ObjectDeleted += (sender, e) => dummy.Test();

            _factory.ClearCreatedObjects();

            dummy.AssertWasCalled(x=> x.Test(), options => options.Repeat.Twice());
        }
    }
}