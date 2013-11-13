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
        public void Should_Populate_Properties()
        {
            _plant.Define(() => new House() { Color = "Red" });

            var house = _plant.Create<House>();

            Assert.AreEqual("Red", house.Color);
        }

        [Test]
        public void Should_Override_With_Custom_Properties()
        {
            _plant.Define(() => new House() { Color = "Red" });

            var house = _plant.Create<House>(x => x.Color = "Green");

            Assert.AreEqual("Green", house.Color);
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

        //[Test]
        //public void Is_Event_Created_Called()
        //{
        //    plant.DefinePropertiesOf(new House() { Color = "blue", SquareFoot = 50 });
        //    plant.DefinePropertiesOf(new Person() { FirstName = "Leo" });

        //    plant.BluePrintCreated += plant_BluePrintCreated;
        //    plant.Create<House>();
        //    plant.Create<Person>();
        //}

        //void plant_BluePrintCreated(object sender, BluePrintEventArgs e)
        //{
        //    Assert.IsNotNull(e.ObjectConstructed);
        //}

        //[Test]
        //public void Should_Prefill_Relation()
        //{
        //    plant.DefinePropertiesOf<House>(new House() { Color = "blue", SquareFoot = 50 });
        //    plant.DefinePropertiesOf<Person>(new Person() { FirstName = "Leo" });

        //    var house = plant.Create<House>();
        //    var person = plant.Create<Person>();

        //    Assert.IsNotNull(person.HouseWhereILive);
        //    Assert.AreEqual(house, person.HouseWhereILive);
        //    Assert.AreEqual(house.Color, person.HouseWhereILive.Color);
        //    Assert.AreEqual(house.SquareFoot, person.HouseWhereILive.SquareFoot);
        //}

        //[Test]
        //public void Should_Not_Prefill_Relation_Defined()
        //{
        //    plant.DefinePropertiesOf(new House() { Color = "blue", SquareFoot = 50 });
        //    plant.DefinePropertiesOf(new Person() { FirstName = "Leo", HouseWhereILive = new House() { Color = "Violet" } });

        //    var house = plant.Create<House>();
        //    var person = plant.Create<Person>();

        //    Assert.AreEqual("Violet", person.HouseWhereILive.Color);
        //}

        //[Test]
        //public void Should_Build_Relation()
        //{
        //    plant.DefinePropertiesOf(new House() { Color = "blue", SquareFoot = 50 });
        //    plant.DefinePropertiesOf(new Person() { FirstName = "Leo" });

        //    var person = plant.Build<Person>();

        //    Assert.IsNotNull(person.HouseWhereILive);
        //}

        //[Test]
        //public void Should_Create_Variation_Of_Specified_Type()
        //{
        //    plant.DefinePropertiesOf<Person>(new { FirstName = "" });
        //    plant.DefineVariationOf<Person>("My", new { FirstName = "My" });
        //    plant.DefineVariationOf<Person>("Her", new { FirstName = "Her" });

        //    Assert.IsInstanceOf(typeof(Person), plant.Create<Person>());
        //    Assert.IsInstanceOf(typeof(Person), plant.Create<Person>("My"));
        //    Assert.IsInstanceOf(typeof(Person), plant.Create<Person>("Her"));
        //}

        //[Test]
        //public void Should_Create_Variation_With_Extension()
        //{
        //    plant.DefinePropertiesOf<House>(new House { Color = "blue" }, OnPropertyPopulation);
        //    plant.DefineVariationOf<House>("My", new House { Color = "My" }, OnPropertyPopulationVariation);

        //    Assert.AreEqual(plant.Create<House>().Persons.First().FirstName, "Pablo");
        //    Assert.AreEqual(plant.Create<House>("My").Persons.First().FirstName, "Pedro");
        //}

        //private static void OnPropertyPopulation(House h)
        //{
        //    h.Persons.Add(new Person() { FirstName = "Pablo" });
        //}

        //private static void OnPropertyPopulationVariation(House h)
        //{
        //    h.Persons.Clear();
        //    h.Persons.Add(new Person() { FirstName = "Pedro" });
        //}

        //[Test]
        //public void Should_Create_Variation_Of_Specified_Type_With_Correct_Data()
        //{
        //    plant.DefinePropertiesOf<Person>(new { FirstName = "" });
        //    plant.DefineVariationOf<Person>("My", new { FirstName = "My" });

        //    var person = plant.Create<Person>("My");
        //    Assert.AreEqual("My", person.FirstName);
        //}


        //[Test]
        //public void Should_Create_Instance_With_Requested_Properties()
        //{
        //    plant.DefinePropertiesOf<Person>(new { FirstName = "" });
        //    Assert.AreEqual("James", plant.Create<Person>(new { FirstName = "James" }).FirstName);
        //}

        //[Test]
        //public void Should_Use_Default_Instance_Values()
        //{
        //    plant.DefinePropertiesOf<Person>(new { FirstName = "Barbara" });
        //    Assert.AreEqual("Barbara", plant.Create<Person>().FirstName);
        //}

        //[Test]
        //public void Should_Create_Instance_With_Null_Value()
        //{
        //    plant.DefinePropertiesOf<Person>(new { FirstName = "Barbara", LastName = (string)null });
        //    Assert.IsNull(plant.Create<Person>().LastName);
        //}

        //[Test]
        //public void Should_Create_Instance_With_Default_Properties_Specified_By_Instance()
        //{
        //    plant.DefinePropertiesOf(new Person { FirstName = "James" });
        //    Assert.AreEqual("James", plant.Create<Person>().FirstName);
        //}

        //[Test]
        //public void Should_Create_Instance_With_Requested_Properties_Specified_By_Instance()
        //{
        //    plant.DefinePropertiesOf(new Person { FirstName = "David" });
        //    Assert.AreEqual("James", plant.Create(new Person { FirstName = "James" }).FirstName);
        //}

        //[Test]
        //[ExpectedException(typeof(PropertyNotFoundException))]
        //public void Should_Throw_PropertyNotFound_Exception_When_Given_Invalid_Property()
        //{
        //    plant.DefinePropertiesOf<Person>(new { Foo = "" });
        //    plant.Create<Person>();
        //}

        [Test]
        [ExpectedException(typeof(TypeNotSetupException))]
        public void Should_Throw_TypeNotSetupException_When_Trying_To_Create_Type_That_Is_Not_Setup()
        {
            _plant.Create<Person>(z => z.FirstName = "Barbara");
        }

        //[Test]
        //[ExpectedException(typeof(ConstructorNotFoundException))]
        //public void Should_Throw_ConstructorNotFoundException_When_Constructor_Given_Invalid_Parameters()
        //{
        //    plant.DefineConstructionOf<Person>(new { FirstName = "Barbara" });
        //    plant.Create<Person>();
        //}


        //[Test]
        //public void Should_Set_User_Properties_That_Are_Not_Defaulted()
        //{
        //    plant.DefinePropertiesOf<Person>(new { FirstName = "Barbara" });
        //    Assert.AreEqual("Brechtel", plant.Create<Person>(new { LastName = "Brechtel" }).LastName);
        //}

        //[Test]
        //public void Should_Lazily_Evaluate_Delegate_Properties()
        //{
        //    string lazyMiddleName = null;
        //    plant.DefinePropertiesOf<Person>(new
        //                           {
        //                               MiddleName = new LazyProperty<string>(() => lazyMiddleName)
        //                           });

        //    Assert.AreEqual(null, plant.Create<Person>().MiddleName);
        //    lazyMiddleName = "Johnny";
        //    Assert.AreEqual("Johnny", plant.Create<Person>().MiddleName);
        //}

        //[Test]
        //[ExpectedException(typeof(LazyPropertyHasWrongTypeException))]
        //public void Should_Throw_LazyPropertyHasWrongTypeException_When_Lazy_Property_Definition_Returns_Wrong_Type()
        //{
        //    plant.DefinePropertiesOf<Person>(new
        //    {
        //        MiddleName = new LazyProperty<int>(() => 5)
        //    });

        //    plant.Create<Person>();
        //}

        //[Test]
        //public void Should_Create_Objects_Via_Constructor()
        //{
        //    plant.DefineConstructionOf<Car>(new { Make = "Toyota" });
        //    Assert.AreEqual("Toyota", plant.Create<Car>().Make);
        //}

        //[Test]
        //public void Should_Send_Constructor_Arguments_In_Correct_Order()
        //{
        //    plant.DefineConstructionOf<Book>(new { Publisher = "Tor", Author = "Robert Jordan" });
        //    Assert.AreEqual("Tor", plant.Create<Book>().Publisher);
        //    Assert.AreEqual("Robert Jordan", plant.Create<Book>().Author);
        //}

        //[Test]
        //public void Should_Override_Default_Constructor_Arguments()
        //{
        //    plant.DefineConstructionOf<House>(new { Color = "Red", SquareFoot = 3000 });

        //    Assert.AreEqual("Blue", plant.Create<House>(new { Color = "Blue" }).Color);
        //}

        //[Test]
        //public void Should_Only_Set_Properties_Once()
        //{
        //    plant.DefinePropertiesOf<WriteOnceMemoryModule>(new { Value = 5000 });
        //    Assert.AreEqual(10, plant.Create<WriteOnceMemoryModule>(new { Value = 10 }).Value);
        //}

        //[Test]
        //public void Should_Call_AfterBuildCallback_After_Properties_Populated()
        //{
        //    plant.DefinePropertiesOf(new Person { FirstName = "Angus", LastName = "MacGyver" },
        //        (p) => p.FullName = p.FirstName + p.LastName);
        //    var builtPerson = plant.Create<Person>();
        //    Assert.AreEqual(builtPerson.FullName, "AngusMacGyver");
        //}

        ////[Test]
        ////public void Should_Call_AfterBuildCallback_After_Constructor_Population()
        ////{
        ////    plant.DefineConstructionOf<House>(new { Color = "Red", SquareFoot = 3000 },
        ////        (h) => h.Summary = h.Color + h.SquareFoot);

        ////    Assert.AreEqual("Blue3000", plant.Create<House>(new { Color = "Blue" }).Summary);
        ////}

        //[Test]
        //public void Should_Create_Objects_In_AfterBuildCallback_After_Properties_Populated()
        //{
        //    plant.DefinePropertiesOf(new House());
        //    plant.DefinePropertiesOf(new Person { FirstName = "Angus", LastName = "MacGyver" },
        //        (p) => p.HouseWhereILive = plant.Create<House>(new House() { Color = "Red" }));
        //    var builtPerson = plant.Create<Person>();

        //    Assert.NotNull(builtPerson.HouseWhereILive);
        //    Assert.AreEqual(builtPerson.HouseWhereILive.Color, "Red");
        //}

        //[Test]
        //public void Should_Create_Objects_In_AfterBuildCallback_After_Constructors_Populated()
        //{
        //    plant.DefinePropertiesOf(new Person());
        //    plant.DefineConstructionOf<House>(new { Color = "Red", SquareFoot = 1 },
        //        (p) => p.Persons = new[] { plant.Create<Person>(new Person() { FirstName = "John" }) });
        //    var buildHouse = plant.Create<House>();

        //    Assert.NotNull(buildHouse.Persons.First());
        //    Assert.AreEqual(buildHouse.Persons.First().FirstName, "John");
        //}



        //[Test]
        //public void Should_increment_values_in_a_sequence_with_property_construction()
        //{
        //    plant.DefinePropertiesOf<Person>(new
        //      {
        //          FirstName = new Sequence<string>((i) => "FirstName" + i)
        //      });
        //    Assert.AreEqual("FirstName0", plant.Create<Person>().FirstName);
        //    Assert.AreEqual("FirstName1", plant.Create<Person>().FirstName);
        //}

        //[Test]
        //public void Should_increment_values_in_a_sequence_with_ctor_construction()
        //{
        //    plant.DefineConstructionOf<House>(new
        //    {
        //        Color = new Sequence<string>((i) => "Color" + i),
        //        SquareFoot = 10
        //    });
        //    Assert.AreEqual("Color0", plant.Create<House>().Color);
        //    Assert.AreEqual("Color1", plant.Create<House>().Color);
        //}

    }

}

