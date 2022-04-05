using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZoDream.Tests
{
    [TestClass]
    public class HexTest
    {
        [TestMethod]
        public void TestEnum()
        {
            Assert.AreEqual(BaseMode.Hex + 1, BaseMode.Binary);
        }
    }

    public enum BaseMode
    {
        Binary,
        Octal,
        Decimal,
        Hex
    }
}