using System;
using System.Linq;
using NUnit.Framework;
using Plant.Core;
using Plant.Core.Exceptions;
using Plant.Core.Helpers;
using Plant.Core.Impl;
using Plant.Tests.TestModels;
using Rhino.Mocks;

namespace Plant.Tests
{
    [TestFixture]
    public class BasePlantTest
    {
        public interface IDummy
        {
            void Test();
        }

        private IPlant _plant;

        [SetUp]
        public void SetUp()
        {
            _plant = PlantFarm.Cultivate();
        }

        [Test]
        public void Should_Create_Instance_Of_Specified_Type()
        {
            _plant.Define(() => new Person());

            Assert.IsInstanceOf(typeof(Person), _plant.Create<Person>());
        }

        [Test]
        [ExpectedException(typeof(WrongDefinitionTypeException))]
        public void Should_Throw_Exception_If_Not_An_MemberInit_Or_New_Function()
        {
            _plant.Define(() => string.Empty);
        }

        [Test]
        public void Should_Create_By_Constructor_With_Parameters()
        {
            _plant.Define(() => new Car("make"));

            var car = _plant.Create<Car>();

            Assert.AreEqual("make", car.Make);
        }

        [Test]
        public void Should_Use_Default_Instance_Values()
        {
            _plant.Define(() => new Person { FirstName = "Barbara" });
            Assert.AreEqual("Barbara", _plant.Create<Person>().FirstName);
        }

        [Test]
        public void Should_Create_Instance_With_Requested_Properties()
        {
            _plant.Define(() => new Person { FirstName = "" });
            Assert.AreEqual("James", _plant.Create<Person>(p => p.FirstName = "James").FirstName);
        }

        [Test]
        public void Can_Create_Two_Different_House()
        {
            _plant.Define(() => new House() { Color = "blue", SquareFoot = 50 });
            _plant.Define(() => new Person() { FirstName = "Leo" });

            var house = _plant.Create<House>();
            var redHouse = _plant.Create<House>(h => h.Color = "red");

            Assert.AreNotEqual(house, redHouse);
            Assert.AreNotEqual(house.Color, redHouse.Color);
        }

        [Test]
        public void Should_Not_Evaluate_Property_Without_Setter()
        {
            Assert.DoesNotThrow(() => _plant.Define(() => new Dog() { Name = "Bob" }));
        }

        [Test]
        public void Should_Not_Try_To_Populate_Property_Without_Setter()
        {
            _plant.Define(() => new Dog() { Name = "Bob" });

            Assert.DoesNotThrow(() => _plant.Create<Dog>());
        }

        [Test]
        public void Is_Event_Created_Called()
        {
            var dummy = MockRepository.GenerateStub<IDummy>();
            dummy.Expect(d => d.Test()).Repeat.Twice();

            _plant.Define(() => new House() { Color = "blue", SquareFoot = 50 });
            _plant.Define(() => new Person() { FirstName = "Leo" });

            _plant.BluePrintCreated += (sender, e) => dummy.Test();
            _plant.Create<House>();
            _plant.Create<Person>();

            dummy.VerifyAllExpectations();
        }

        [Test]
        public void Should_Prefill_Relation()
        {
            _plant.Define(() => new House() { Color = "blue", SquareFoot = 50 });
            _plant.Define(() => new Person() { FirstName = "Leo" });

            var house = _plant.Create<House>();
            var person = _plant.Create<Person>();

            Assert.IsNotNull(person.HouseWhereILive);
            Assert.AreEqual(house, person.HouseWhereILive);
            Assert.AreEqual(house.Color, person.HouseWhereILive.Color);
            Assert.AreEqual(house.SquareFoot, person.HouseWhereILive.SquareFoot);
        }

        [Test]
        public void Should_Not_Prefill_Relation_Defined()
        {
            _plant.Define(() => new House() { Color = "blue", SquareFoot = 50 });
            _plant.Define(() => new Person() { FirstName = "Leo", HouseWhereILive = new House() { Color = "Violet" } });

            var person = _plant.Create<Person>();

            Assert.AreEqual("Violet", person.HouseWhereILive.Color);
        }

        [Test]
        public void Should_Build_Relation()
        {
            _plant.Define(() => new House() { Color = "blue", SquareFoot = 50 });
            _plant.Define(() => new Person() { FirstName = "Leo" });

            var person = _plant.Build<Person>();

            Assert.IsNotNull(person.HouseWhereILive);
        }

        #region Variations

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
        public void Should_Create_Variation_With_Extension()
        {
            _plant.Define(() => new House { Color = "blue" }, OnPropertyPopulation);
            _plant.Define("My", () => new House { Color = "My" }, OnPropertyPopulationVariation);

            Assert.AreEqual(_plant.Create<House>().Persons.First().FirstName, "Pablo");
            Assert.AreEqual(_plant.Create<House>("My").Persons.First().FirstName, "Pedro");
        }

        private static void OnPropertyPopulation(House h)
        {
            h.Persons.Add(new Person() { FirstName = "Pablo" });
        }

        private static void OnPropertyPopulationVariation(House h)
        {
            h.Persons.Clear();
            h.Persons.Add(new Person() { FirstName = "Pedro" });
        }

        [Test]
        public void Should_Create_Variation_Of_Specified_Type_With_Correct_Data()
        {
            _plant.Define(() => new Person() { FirstName = "" });
            _plant.Define("My", () => new Person { FirstName = "My" });

            var person = _plant.Create<Person>("My");
            Assert.AreEqual("My", person.FirstName);
        }

        #endregion

        [Test]
        public void Should_Create_Instance_With_Null_Value()
        {
            _plant.Define(() => new Person { FirstName = "Barbara", LastName = (string)null });
            Assert.IsNull(_plant.Create<Person>().LastName);
        }

        [Test]
        [ExpectedException(typeof(TypeNotSetupException))]
        public void Should_Throw_TypeNotSetupException_When_Trying_To_Create_Type_That_Is_Not_Setup()
        {
            _plant.Create<Person>(z => z.FirstName = "Barbara");
        }

        [Test]
        public void Should_Set_User_Properties_That_Are_Not_Defaulted()
        {
            _plant.Define(() => new Person() { FirstName = "Barbara" });
            Assert.AreEqual("Brechtel", _plant.Create<Person>(p => p.LastName = "Brechtel").LastName);
        }

        [Test]
        public void Should_Lazily_Evaluate_Properties()
        {
            string lazyMiddleName = null;
            _plant.Define(() => new Person()
                                   {
                                       MiddleName = lazyMiddleName
                                   });

            Assert.AreEqual(null, _plant.Create<Person>().MiddleName);
            lazyMiddleName = "Johnny";
            Assert.AreEqual("Johnny", _plant.Create<Person>().MiddleName);
        }

        [Test]
        public void Should_Override_Default_Constructor_Arguments()
        {
            _plant.Define(() => new House { Color = "Red", SquareFoot = 3000 });

            Assert.Fail();
            //Assert.AreEqual("Blue", _plant.Create<House>(x =>  x.Color = "Blue").Color);
        }

        [Test]
        public void Should_Only_Set_Properties_Once()
        {
            _plant.Define(() => new WriteOnceMemoryModule { Value = 5000 });
            Assert.AreEqual(10, _plant.Create<WriteOnceMemoryModule>(x => x.Value = 10).Value);
        }

        [Test]
        public void Should_Call_AfterCreationCallback_After_Creation()
        {
            _plant.Define(() => new Person { FirstName = "Angus", LastName = "MacGyver" },
                (p) => p.FullName = p.FirstName + p.LastName);
            var builtPerson = _plant.Create<Person>();
            Assert.AreEqual("AngusMacGyver", builtPerson.FullName);
        }

        [Test]
        public void Should_Call_AfterCreationCallback_After_Building()
        {
            _plant.Define(() => new Person { FirstName = "Angus", LastName = "MacGyver" },
                (p) => p.FullName = p.FirstName + p.LastName);
            var builtPerson = _plant.Build<Person>();
            Assert.AreEqual(null, builtPerson.FullName);
        }

        [Test]
        public void Should_Create_Objects_In_AfterCreationCallback()
        {
            _plant.Define(() => new House());
            _plant.Define(() => new Person { FirstName = "Angus", LastName = "MacGyver" },
                (p) => p.HouseWhereILive = _plant.Create<House>(x => x.Color = "Red"));
            var builtPerson = _plant.Create<Person>();

            Assert.NotNull(builtPerson.HouseWhereILive);
            Assert.AreEqual(builtPerson.HouseWhereILive.Color, "Red");
        }

        [Test]
        public void Should_increment_values_in_a_sequence()
        {
            _plant.Define(() => new Person()
              {
                  FirstName = Sequence.Evaluate((i) => "FirstName" + i)
              });
            Assert.AreEqual("FirstName0", _plant.Create<Person>().FirstName);
            Assert.AreEqual("FirstName1", _plant.Create<Person>().FirstName);
        }
    }
}

