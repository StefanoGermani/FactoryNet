using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Plant.Core;
using Plant.Core.Exceptions;
using Plant.Tests.TestModels;
using Rhino.Mocks;

namespace Plant.Tests
{
    [TestFixture]
    public class PersisterPlantTests
    {
        private IPersisterSeed _persister;

        [SetUp]
        public void SetUp()
        {
            _persister = MockRepository.GenerateMock<IPersisterSeed>();
        }

        [Test]
        public void Should_Call_Persister_Save_Method_When_Creating_Objects()
        {
            _persister.Expect(a => a.Save(Arg<House>.Is.Anything)).Return(true);

            var plant = new PersisterPlant(_persister);
            plant.DefinePropertiesOf(new House() { Color = "blue", SquareFoot = 50 });
            plant.Create<House>();

            _persister.VerifyAllExpectations();
        }

        [Test]
        public void Should_Not_Call_Persister_Save_Method_When_Building_Objects()
        {
            var plant = new PersisterPlant(_persister);
            plant.DefinePropertiesOf(new House() { Color = "blue", SquareFoot = 50 });
            plant.Build<House>();

            _persister.AssertWasNotCalled(p => p.Save(Arg<House>.Is.Anything));
        }

        [Test]
        [ExpectedException(typeof(PersisterException))]
        public void Should_Throw_PersisterException_If_Persister_Is_Null()
        {
            var plant = new PersisterPlant(null);
        }

        [Test]
        [ExpectedException(typeof(PersisterException))]
        public void Should_Throw_PersisterException_If_Persister_Cant_Save()
        {
            _persister.Stub(a => a.Save(Arg<House>.Is.Anything)).Return(false);

            var plant = new PersisterPlant(_persister);
            plant.DefinePropertiesOf(new House() { Color = "blue", SquareFoot = 50 });
            plant.Create<House>();
        }

        [Test]
        [ExpectedException(typeof(PersisterException))]
        public void Should_Throw_PersisterException_If_Persister_Throw_Exception()
        {
            _persister.Stub(a => a.Save(Arg<House>.Is.Anything)).Throw(new Exception());

            var plant = new PersisterPlant(_persister);
            plant.DefinePropertiesOf(new House() { Color = "blue", SquareFoot = 50 });
            plant.Create<House>();
        }
    }
}
