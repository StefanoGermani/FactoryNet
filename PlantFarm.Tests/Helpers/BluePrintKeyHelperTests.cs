using NUnit.Framework;
using PlantFarm.Core.Helpers;
using PlantFarm.Tests.TestModels;

namespace PlantFarm.Tests.Helpers
{
    [TestFixture]
    public class BluePrintKeyHelperTests
    {
        private BluePrintKeyHelper _bluePrintKeyHelper;

        [SetUp]
        public void Setup()
        {
            _bluePrintKeyHelper = new BluePrintKeyHelper();
        }

        [Test]
        public void ShouldBuildRightKey()
        {
            var bookKey = _bluePrintKeyHelper.GetBluePrintKey<Book>(string.Empty);

            Assert.AreEqual("PlantFarm.Tests.TestModels.Book-", bookKey);
        }

        [Test]
        public void ShouldBuildRightKeyWithVariation()
        {
            var bookKey = _bluePrintKeyHelper.GetBluePrintKey<Book>("variation");

            Assert.AreEqual("PlantFarm.Tests.TestModels.Book-variation", bookKey);
        }

        [Test]
        public void ShouldBuildRightKeyWithVariationAndType()
        {
            var bookKey = _bluePrintKeyHelper.GetBluePrintKey("variation", typeof(Book));

            Assert.AreEqual("PlantFarm.Tests.TestModels.Book-variation", bookKey);
        }
    }
}
