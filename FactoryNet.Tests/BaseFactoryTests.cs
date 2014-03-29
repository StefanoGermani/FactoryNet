using System;
using System.Globalization;
using System.Reflection;
using FactoryNet.Core;
using FactoryNet.Core.Exceptions;
using FactoryNet.Tests.TestModels;
using NUnit.Framework;
using Rhino.Mocks;

namespace FactoryNet.Tests
{
    [TestFixture]
    public class BaseFactoryTests
    {
        [SetUp]
        public void SetUp()
        {
            _factory = new BaseFactory();
        }

        private IFactory _factory;

        [Test]
        public void Can_Create_Two_Different_House()
        {
            _factory.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _factory.Define(() => new Person { FirstName = "Leo" });

            var house = _factory.Create<House>();
            var redHouse = _factory.Create<House>(h => h.Color = "red");

            Assert.AreNotEqual(house, redHouse);
            Assert.AreNotEqual(house.Color, redHouse.Color);
        }

        [Test]
        public void Is_Event_Created_Called()
        {
            var dummy = MockRepository.GenerateStub<IDummy>();
            dummy.Expect(d => d.Test()).Repeat.Twice();

            _factory.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _factory.Define(() => new Person { FirstName = "Leo" });

            _factory.ObjectCreated += (sender, e) => dummy.Test();
            _factory.Create<House>();
            _factory.Create<Person>();

            dummy.VerifyAllExpectations();
        }

        [Test]
        public void Should_Build_Relation()
        {
            _factory.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _factory.Define(() => new Person { FirstName = "Leo" });

            var person = _factory.Build<Person>();

            Assert.IsNotNull(person.HouseWhereILive);
        }

        [Test]
        public void Should_Call_AfterCreationCallback_After_Building()
        {
            _factory.Define(() => new Person { FirstName = "Angus", LastName = "MacGyver" },
                          p => p.FullName = p.FirstName + p.LastName);
            var builtPerson = _factory.Build<Person>();
            Assert.AreEqual(null, builtPerson.FullName);
        }

        [Test]
        public void Should_Call_AfterCreationCallback_After_Creation()
        {
            _factory.Define(() => new Person { FirstName = "Angus", LastName = "MacGyver" },
                          p => p.FullName = p.FirstName + p.LastName);
            var builtPerson = _factory.Create<Person>();
            Assert.AreEqual("AngusMacGyver", builtPerson.FullName);
        }

        [Test]
        public void Should_Create_By_Constructor_With_Parameters()
        {
            _factory.Define(() => new Car("make"));

            var car = _factory.Create<Car>();

            Assert.AreEqual("make", car.Make);
        }

        [Test]
        public void Should_Create_Instance_Of_Specified_Type()
        {
            _factory.Define(() => new Person());

            Assert.IsInstanceOf(typeof(Person), _factory.Create<Person>());
        }

        [Test]
        public void Should_Create_Instance_With_Null_Value()
        {
            _factory.Define(() => new Person { FirstName = "Barbara", LastName = (string)null });
            Assert.IsNull(_factory.Create<Person>().LastName);
        }

        [Test]
        public void Should_Create_Instance_With_Not_Constant_Value()
        {
            _factory.Define(() => new House(new Random().Next().ToString(CultureInfo.InvariantCulture), new Random().Next()));
            Assert.IsNotNull(_factory.Create<House>());
        }

        [Test]
        public void Should_Create_Instance_With_Requested_Properties()
        {
            _factory.Define(() => new Person { FirstName = "" });
            Assert.AreEqual("James", _factory.Create<Person>(p => p.FirstName = "James").FirstName);
        }

        [Test]
        public void Should_Create_Objects_In_AfterCreationCallback()
        {
            _factory.Define(() => new House());
            _factory.Define(() => new Person { FirstName = "Angus", LastName = "MacGyver" },
                          p => p.HouseWhereILive = _factory.Create<House>(x => x.Color = "Red"));
            var builtPerson = _factory.Create<Person>();

            Assert.NotNull(builtPerson.HouseWhereILive);
            Assert.AreEqual(builtPerson.HouseWhereILive.Color, "Red");
        }

        [Test]
        public void Should_Lazily_Evaluate_Properties()
        {
            string lazyMiddleName = null;
            _factory.Define(() => new Person
                {
                    MiddleName = lazyMiddleName
                });

            Assert.AreEqual(null, _factory.Create<Person>().MiddleName);
            lazyMiddleName = "Johnny";
            Assert.AreEqual("Johnny", _factory.Create<Person>().MiddleName);
        }

        [Test]
        public void Should_Not_Evaluate_Property_Without_Setter()
        {
            Assert.DoesNotThrow(() => _factory.Define(() => new Dog { Name = "Bob" }));
        }

        [Test]
        public void Should_Not_Prefill_Relation_Defined()
        {
            _factory.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _factory.Define(() => new Person { FirstName = "Leo", HouseWhereILive = new House { Color = "Violet" } });

            var person = _factory.Create<Person>();

            Assert.AreEqual("Violet", person.HouseWhereILive.Color);
        }

        [Test]
        public void Should_Not_Try_To_Populate_Property_Without_Setter()
        {
            _factory.Define(() => new Dog { Name = "Bob" });

            Assert.DoesNotThrow(() => _factory.Create<Dog>());
        }

        [Test, Ignore("Can't do it on 3.5 .Net Framework")]
        public void Should_Only_Set_Properties_Once()
        {
            _factory.Define(() => new WriteOnceMemoryModule { Value = 5000 });
            Assert.AreEqual(10, _factory.Create<WriteOnceMemoryModule>(x => x.Value = 10).Value);
        }

        //[Test]
        //public void Should_Override_Default_Constructor_Arguments()
        //{
        //    _factory.Define(() => new House { Color = "Red", SquareFoot = 3000 });

        //    Assert.AreEqual("Blue", _factory.Create(() => new House("Blue", 5)).Color);
        //}

        //[Test]
        //public void Should_Override_Default_Properties()
        //{
        //    _factory.Define(() => new House { Color = "Red", SquareFoot = 3000 });

        //    Assert.AreEqual("Blue", _factory.Create(() => new House() { Color = "Blue" }).Color);
        //}

        [Test]
        public void Should_Prefill_Relation()
        {
            _factory.Define(() => new House { Color = "blue", SquareFoot = 50 });
            _factory.Define(() => new Person { FirstName = "Leo" });

            var house = _factory.Create<House>();
            var person = _factory.Create<Person>();

            Assert.IsNotNull(person.HouseWhereILive);
            Assert.AreEqual(house, person.HouseWhereILive);
            Assert.AreEqual(house.Color, person.HouseWhereILive.Color);
            Assert.AreEqual(house.SquareFoot, person.HouseWhereILive.SquareFoot);
        }

        [Test]
        public void Should_Set_User_Properties_That_Are_Not_Defaulted()
        {
            _factory.Define(() => new Person { FirstName = "Barbara" });
            Assert.AreEqual("Brechtel", _factory.Create<Person>(p => p.LastName = "Brechtel").LastName);
        }

        [Test]
        [ExpectedException(typeof(WrongDefinitionTypeException))]
        public void Should_Throw_Exception_If_Not_An_MemberInit_Or_New_Function()
        {
            _factory.Define(() => string.Empty);
        }

        [Test]
        [ExpectedException(typeof(TypeNotSetupException))]
        public void Should_Throw_TypeNotSetupException_When_Trying_To_Create_Type_That_Is_Not_Setup()
        {
            _factory.Create<Person>(z => z.FirstName = "Barbara");
        }

        [Test]
        public void Should_Use_Default_Instance_Values()
        {
            _factory.Define(() => new Person { FirstName = "Barbara" });
            Assert.AreEqual("Barbara", _factory.Create<Person>().FirstName);
        }

        [Test]
        public void Should_increment_values_in_a_sequence()
        {
            _factory.Define(() => new Person
                {
                    FirstName = Sequence.Evaluate(i => string.Format("FirstName{0}", i))
                });
            Assert.AreEqual("FirstName0", _factory.Create<Person>().FirstName);
            Assert.AreEqual("FirstName1", _factory.Create<Person>(x => x.LastName = "LastName").FirstName);
        }

        [Test]
        public void Should_increment_values_in_three_sequences()
        {
            _factory.Define(() => new Person
            {
                FirstName = Sequence.Evaluate(i => "FirstName" + i),
                LastName = Sequence.Evaluate(i => "LastName" + i),
                Age = Sequence.Evaluate(i => i)
            });

            var person = _factory.Create<Person>();

            Assert.AreEqual("FirstName0", person.FirstName);
            Assert.AreEqual("LastName0", person.LastName);
            Assert.AreEqual(0, person.Age);
        }

        [Test]
        public void Should_Load_Blueprint_From_Assembly()
        {
            _factory.LoadBlueprintsFromAssembly(Assembly.GetExecutingAssembly());

            Assert.DoesNotThrow(() => _factory.Create<House>());
        }

        [Test]
        public void Should_Load_Blueprint_From_Current_Assembly()
        {
            _factory.LoadBlueprintsFromCurrentAssembly();

            Assert.DoesNotThrow(() => _factory.Create<House>());
        }

        [Test]
        public void Should_Load_Blueprint_From_Loaded_Assemblies()
        {
            _factory.LoadBlueprintsFromAssemblies();

            Assert.DoesNotThrow(() => _factory.Create<House>());
        }

    }
}