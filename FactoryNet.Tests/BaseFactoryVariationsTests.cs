using System.Linq;
using FactoryNet.Core;
using FactoryNet.Tests.TestModels;
using NUnit.Framework;

namespace FactoryNet.Tests
{
    [TestFixture]
    public class BaseFactoryVariationsTests
    {
        private IFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _factory = new BaseFactory();
        }

        [Test]
        public void Should_Create_Variation_Of_Specified_Type()
        {
            _factory.Define(() => new Person { FirstName = "" });
            _factory.Define("My", () => new Person { FirstName = "My" });
            _factory.Define("Her", () => new Person { FirstName = "Her" });

            Assert.IsInstanceOf(typeof(Person), _factory.Create<Person>());
            Assert.IsInstanceOf(typeof(Person), _factory.Create<Person>("My"));
            Assert.IsInstanceOf(typeof(Person), _factory.Create<Person>("Her"));
        }

        [Test]
        public void Should_Create_Variation_Of_Specified_Type_With_Correct_Data()
        {
            _factory.Define(() => new Person { FirstName = "" });
            _factory.Define("My", () => new Person { FirstName = "My" });

            var person = _factory.Create<Person>("My");
            Assert.AreEqual("My", person.FirstName);
        }

        [Test]
        public void Should_Create_Variation_With_Extension()
        {
            _factory.Define(() => new House { Color = "blue" }, h => h.Persons.Add(new Person { FirstName = "Pablo" }));
            _factory.Define("My", () => new House { Color = "My" }, h =>
            {
                h.Persons.Clear();
                h.Persons.Add(new Person { FirstName = "Pedro" });
            });

            Assert.AreEqual(_factory.Create<House>().Persons.First().FirstName, "Pablo");
            Assert.AreEqual(_factory.Create<House>("My").Persons.First().FirstName, "Pedro");
        }

    }
}
