using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ICG = IdentCodes.IdentCodeGenerator;

namespace IdentCodeTests
{
    [TestClass]
    public class CodeGeneratorTests
    {
        private void TestMethod(string matStr, int expectedCode)
        {
            var code = ICG.GetMaterialCode(matStr);
            Assert.AreEqual(code, expectedCode);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var matStr = "C345";
            var expectedCode = 1;
            TestMethod(matStr, expectedCode);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var matStr = "C355";
            var expectedCode = 0;
            TestMethod(matStr, expectedCode);
        }
    }
}
