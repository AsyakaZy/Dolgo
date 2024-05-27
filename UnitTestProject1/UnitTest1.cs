using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Klevtsov_Zakharov;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
           MainWindow main = new MainWindow();
            var actual = main.Output();
            var expected = "заебись";
            Assert.AreEqual(expected, actual);
        }
    }
}
