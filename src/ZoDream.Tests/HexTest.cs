using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZoDream.HexViewer.Models;
using ZoDream.HexViewer.ViewModels;

namespace ZoDream.Tests
{
    [TestClass]
    public class HexTest
    {
        [TestMethod]
        public void TestEnum()
        {
            var text = "0b00000001";
            var model = new MainViewModel();
            var res = false;
            foreach (var item in model.ByteModeItems)
            {
                if (item.IsMatch(text))
                {
                    res = true;
                }
            }
            Assert.IsTrue(res);
        }
    }

}