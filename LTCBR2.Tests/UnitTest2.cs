using System;
using LTCBR2.Keeper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTCBR2.Tests
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            OwlWorker ow = new OwlWorker();
            ow.LoadIndividuals(@"C:\Users\Рябов\Desktop\dts.owl");
            ow.LoadOntologyModel(@"C:\Users\Рябов\Desktop\dts.owl");
        }
    }
}
