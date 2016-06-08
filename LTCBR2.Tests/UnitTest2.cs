using System;
using System.Linq;
using System.Xml;
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
            XmlDocument xml = new XmlDocument();
            xml.Load(@"C:\Users\Рябов\Desktop\dts.owl");
            OwlWorker.LoadIndividuals(xml);
            var classes = OwlWorker.LoadOntologyModel(xml).Where(x=>x.parent=="#Subject");
        }
    }
}
