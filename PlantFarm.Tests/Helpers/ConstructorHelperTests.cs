using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using NUnit.Framework;
using PlantFarm.Core.Helpers;
using PlantFarm.Tests.TestModels;

namespace PlantFarm.Tests.Helpers
{
    [TestFixture]
    public class ConstructorHelperTests
    {
        private IConstructorHelper _constructorHelper;

        [SetUp]
        public void Setup()
        {
            _constructorHelper = new ConstructorHelper();
        }

        [Test]
        public void Should_Create_Istance()
        {
            Expression<Func<Book>> definition = () => new Book("author", "publisher");
            var newExpression = ((NewExpression) definition.Body);

            var istance = _constructorHelper.CreateInstance<Book>(newExpression);

            Assert.IsInstanceOf<Book>(istance);
            Assert.AreEqual("author", istance.Author);
        }
    }
}
