using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Plant.Core;
using Plant.Core.Exceptions;
using Plant.Core.Impl;
using Plant.Tests.TestModels;
using Rhino.Mocks;

namespace Plant.Tests
{
    [TestFixture]
    public class PersisterPlantTests
    {
        private IPersisterSeed _persister;
        private IPlant plant;

        [SetUp]
        public void SetUp()
        {
            _persister = MockRepository.GenerateMock<IPersisterSeed>();
            plant = PlantFarm.Cultivate(_persister);
        }

        [Test]
        public void Should_Call_Persister_Save_Method_When_Creating_Objects()
        {
            _persister.Expect(a => a.Save(Arg<object>.Is.Anything)).Return(true);


            plant.Define(() => new House() { Color = "blue", SquareFoot = 50 });
            plant.Create<House>();

            _persister.VerifyAllExpectations();
        }

        [Test]
        public void Should_Not_Call_Persister_Save_Method_When_Building_Objects()
        {

            plant.Define(() => new House() { Color = "blue", SquareFoot = 50 });
            plant.Build<House>();

            _persister.AssertWasNotCalled(p => p.Save(Arg<House>.Is.Anything));
        }

        [Test]
        [ExpectedException(typeof(PersisterException))]
        public void Should_Throw_PersisterException_If_Persister_Is_Null()
        {
            PlantFarm.Cultivate(null);
        }

        [Test]
        [ExpectedException(typeof(PersisterException))]
        public void Should_Throw_PersisterException_If_Persister_Cant_Save()
        {
            _persister.Stub(a => a.Save(Arg<House>.Is.Anything)).Return(false);


            plant.Define(() => new House() { Color = "blue", SquareFoot = 50 });
            plant.Create<House>();
        }

        [Test]
        [ExpectedException(typeof(PersisterException))]
        public void Should_Throw_PersisterException_If_Persister_Throw_Exception()
        {
            _persister.Stub(a => a.Save(Arg<House>.Is.Anything)).Throw(new Exception());


            plant.Define(() => new House() { Color = "blue", SquareFoot = 50 });
            plant.Create<House>();
        }
    }
}
