using FactoryNet.Core.Helpers;
using FactoryNet.Tests.TestModels;
using NUnit.Framework;

namespace FactoryNet.Tests.Helpers
{
    [TestFixture]
    public class BluePrintKeyHelperTests
    {
        private IBluePrintKeyHelper _bluePrintKeyHelper;

        [SetUp]
        public void Setup()
        {
            _bluePrintKeyHelper = new BluePrintKeyHelper();
        }

        [Test]
        public void ShouldBuildRightKey()
        {
            var key = _bluePrintKeyHelper.GetBluePrintKey<Book>(string.Empty);

            Assert.AreEqual("FactoryNet.Tests.TestModels.Book-", key);
        }

        [Test]
        public void ShouldBuildRightKeyWithVariation()
        {
            var key = _bluePrintKeyHelper.GetBluePrintKey<Book>("variation");

            Assert.AreEqual("FactoryNet.Tests.TestModels.Book-variation", key);
        }

        [Test]
        public void ShouldBuildRightKeyWithVariationAndType()
        {
            var key = _bluePrintKeyHelper.GetBluePrintKey("variation", typeof(Book));

            Assert.AreEqual("FactoryNet.Tests.TestModels.Book-variation", key);
        }
    }
}
