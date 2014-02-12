using System;
using FactoryNet.Core;
using FactoryNet.Core.Exceptions;
using FactoryNet.Tests.TestModels;
using NUnit.Framework;
using Rhino.Mocks;

namespace FactoryNet.Tests
{
    [TestFixture]
    public class PersisterPlantTests
    {
        [SetUp]
        public void SetUp()
        {
            _persister = MockRepository.GenerateMock<IPersisterSeed>();
            _plant = new PersisterPlant(_persister);
        }

        private IPersisterSeed _persister;
        private IPlant _plant;

        [Test]
        public void Should_Call_Persister_Save_Method_When_Creating_Objects()
        {
            _persister.Expect(a => a.Save(Arg<object>.Is.Anything)).Return(true);


            _plant.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _plant.Create<House>();

            _persister.VerifyAllExpectations();
        }

        [Test]
        public void Should_Not_Call_Persister_Save_Method_When_Building_Objects()
        {
            _plant.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _plant.Build<House>();

            _persister.AssertWasNotCalled(p => p.Save(Arg<object>.Is.Anything));
        }

        [Test]
        [ExpectedException(typeof(PersisterException))]
        public void Should_Throw_PersisterException_If_Persister_Cant_Save()
        {
            _persister.Stub(a => a.Save(Arg<House>.Is.Anything)).Return(false);


            _plant.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _plant.Create<House>();
        }

        [Test]
        [ExpectedException(typeof(PersisterException))]
        public void Should_Throw_PersisterException_If_Persister_Save_Throw_Exception()
        {
            _persister.Stub(a => a.Save(Arg<House>.Is.Anything)).Throw(new Exception());


            _plant.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _plant.Create<House>();
        }

        [Test]
        public void Should__Call_Persister_Delete_Method_When_Clearing_Objects()
        {
            _persister.Stub(a => a.Save(Arg<object>.Is.Anything)).Return(true);
            _persister.Expect(a => a.Delete(Arg<object>.Is.Anything)).Return(true);

            _plant.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _plant.Create<House>();

            _plant.ClearCreatedObjects();

            _persister.VerifyAllExpectations();
        }

        [Test]
        public void Should__Call_Persister_Delete_Method_In_Reverse_Order()
        {
            _persister.Stub(a => a.Save(Arg<object>.Is.Anything)).Return(true);
            _persister.Stub(a => a.Delete(Arg<object>.Is.Anything)).Return(true);

            _plant.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _plant.Define(() => new Person() { FirstName = "Name" });
            var house = _plant.Create<House>();
            var person = _plant.Create<Person>();


            _plant.ClearCreatedObjects();

            _persister.AssertWasCalled(x => x.Delete(person), options => options.Repeat.Once());
            _persister.AssertWasCalled(x => x.Delete(house), options => options.Repeat.Once());
        }

        [Test]
        [ExpectedException(typeof(PersisterException))]
        public void Should_Throw_PersisterException_If_Persister_Cant_Delete()
        {
            _persister.Stub(a => a.Delete(Arg<House>.Is.Anything)).Return(false);


            _plant.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _plant.Create<House>();

            _plant.ClearCreatedObjects();
        }

        [Test]
        [ExpectedException(typeof(PersisterException))]
        public void Should_Throw_PersisterException_If_Persister_Delete_Throw_Exception()
        {
            _persister.Stub(a => a.Delete(Arg<House>.Is.Anything)).Throw(new Exception());


            _plant.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _plant.Create<House>();

            _plant.ClearCreatedObjects();
        }


        [Test]
        [ExpectedException(typeof(PersisterException))]
        public void Should_Throw_PersisterException_If_Persister_Is_Null()
        {
            new PersisterPlant(null);
        }

    }
}