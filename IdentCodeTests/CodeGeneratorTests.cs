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

        [TestMethod]
        public void TestMethod3()
        {
            var matStr = "C355Б-Z35";
            var expectedCode = 6;
            TestMethod(matStr, expectedCode);
        }

        [TestMethod]
        public void TestMethod4()
        {
            var matStr = "C345Б-Z35";
            var expectedCode = 1;
            TestMethod(matStr, expectedCode);
        }

        [TestMethod]
        public void TestMethod5()
        {
            var matStr = "S420-4-AB-Z35";
            var expectedCode = 2;
            TestMethod(matStr, expectedCode);
        }

        [TestMethod]
        public void TestMethod6()
        {
            var matStr = "API5L-Z35";
            var expectedCode = 3;
            TestMethod(matStr, expectedCode);
        }

        [TestMethod]
        public void TestMethod7()
        {
            var matStr = "S355";
            var expectedCode = 4;
            TestMethod(matStr, expectedCode);
        }

        [TestMethod]
        public void TestMethod8()
        {
            var matStr = "C255";
            var expectedCode = 5;
            TestMethod(matStr, expectedCode);
        }
    }
}
