using System;
using System.Linq;
using System.Xml;
using LTCBR2.Keeper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTCBR2.Tests
{
    [TestClass]
    public class OwlTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(@"C:\Users\Рябов\Desktop\dts.owl");
            OwlWorker.LoadIndividuals(xml);
            var subject = OwlWorker.LoadOntologyModel(xml).Where(x=>x.Purpose=="Subject");
            var process = OwlWorker.LoadOntologyModel(xml).Where(x => x.Purpose == "Process");
            var relation = OwlWorker.LoadOntologyModel(xml).Where(x => x.Purpose == "Relation");
        }
    }
}
