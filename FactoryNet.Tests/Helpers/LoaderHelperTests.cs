using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FactoryNet.Core;
using FactoryNet.Core.Helpers;
using FactoryNet.Tests.TestModels;
using NUnit.Framework;

namespace FactoryNet.Tests.Helpers
{
    [TestFixture]
    public class LoaderHelperTests
    {
        private ILoaderHelper _helper;

        [SetUp]
        public void SetUp()
        {
            _helper = new LoaderHelper(new BaseFactory());
        }

        [Test]
        public void Should_Load_Blueprint_From_Assembly()
        {
            var factory = _helper.LoadBlueprintsFromAssembly(Assembly.GetExecutingAssembly());

            Assert.DoesNotThrow(() => factory.Create<House>());
        }

        [Test]
        public void Should_Load_Blueprint_From_Current_Assembly()
        {
            var factory = _helper.LoadBlueprintsFromCurrentAssembly(Assembly.GetExecutingAssembly());

            Assert.DoesNotThrow(() => factory.Create<House>());
        }

        [Test]
        public void Should_Load_Blueprint_From_Loaded_Assemblies()
        {
            var factory = _helper.LoadBlueprintsFromAssemblies();

            Assert.DoesNotThrow(() => factory.Create<House>());
        }
    }
}
