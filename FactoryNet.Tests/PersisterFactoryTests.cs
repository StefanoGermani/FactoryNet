using System;
using FactoryNet.Core;
using FactoryNet.Core.Exceptions;
using FactoryNet.Tests.TestModels;
using NUnit.Framework;
using Rhino.Mocks;

namespace FactoryNet.Tests
{
    [TestFixture]
    public class PersisterFactoryTests
    {
        [SetUp]
        public void SetUp()
        {
            _persister = MockRepository.GenerateMock<IPersisterSeed>();
            _factory = new PersisterFactory(_persister);
        }

        private IPersisterSeed _persister;
        private IFactory _factory;

        [Test]
        public void Should_Call_Persister_Save_Method_When_Creating_Objects()
        {
            _persister.Expect(a => a.Save(Arg<object>.Is.Anything)).Return(true);


            _factory.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _factory.Create<House>();

            _persister.VerifyAllExpectations();
        }

        [Test]
        public void Should_Not_Call_Persister_Save_Method_When_Building_Objects()
        {
            _factory.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _factory.Build<House>();

            _persister.AssertWasNotCalled(p => p.Save(Arg<object>.Is.Anything));
        }

        [Test]
        [ExpectedException(typeof(PersisterException))]
        public void Should_Throw_PersisterException_If_Persister_Cant_Save()
        {
            _persister.Stub(a => a.Save(Arg<House>.Is.Anything)).Return(false);


            _factory.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _factory.Create<House>();
        }

        [Test]
        [ExpectedException(typeof(PersisterException))]
        public void Should_Throw_PersisterException_If_Persister_Save_Throw_Exception()
        {
            _persister.Stub(a => a.Save(Arg<House>.Is.Anything)).Throw(new Exception());


            _factory.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _factory.Create<House>();
        }

        [Test]
        public void Should__Call_Persister_Delete_Method_When_Clearing_Objects()
        {
            _persister.Stub(a => a.Save(Arg<object>.Is.Anything)).Return(true);
            _persister.Expect(a => a.Delete(Arg<object>.Is.Anything)).Return(true);

            _factory.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _factory.Create<House>();

            _factory.ClearCreatedObjects();

            _persister.VerifyAllExpectations();
        }

        [Test]
        public void Should__Call_Persister_Delete_Method_In_Reverse_Order()
        {
            _persister.Stub(a => a.Save(Arg<object>.Is.Anything)).Return(true);
            _persister.Stub(a => a.Delete(Arg<object>.Is.Anything)).Return(true);

            _factory.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _factory.Define(() => new Person() { FirstName = "Name" });
            var house = _factory.Create<House>();
            var person = _factory.Create<Person>();


            _factory.ClearCreatedObjects();

            _persister.AssertWasCalled(x => x.Delete(person), options => options.Repeat.Once());
            _persister.AssertWasCalled(x => x.Delete(house), options => options.Repeat.Once());
        }

        [Test]
        [ExpectedException(typeof(PersisterException))]
        public void Should_Throw_PersisterException_If_Persister_Cant_Delete()
        {
            _persister.Stub(a => a.Delete(Arg<House>.Is.Anything)).Return(false);


            _factory.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _factory.Create<House>();

            _factory.ClearCreatedObjects();
        }

        [Test]
        [ExpectedException(typeof(PersisterException))]
        public void Should_Throw_PersisterException_If_Persister_Delete_Throw_Exception()
        {
            _persister.Stub(a => a.Delete(Arg<House>.Is.Anything)).Throw(new Exception());


            _factory.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _factory.Create<House>();

            _factory.ClearCreatedObjects();
        }


        [Test]
        [ExpectedException(typeof(PersisterException))]
        public void Should_Throw_PersisterException_If_Persister_Is_Null()
        {
            new PersisterFactory(null);
        }

    }
}