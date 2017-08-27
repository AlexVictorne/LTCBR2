using Microsoft.VisualStudio.TestTools.UnitTesting;
using LTCBR2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LTCBR2.Tests
{
    [TestClass()]
    public class SituationGeneratorTests
    {
        [TestMethod()]
        public void GenerateSituationListTest()
        {
            var xml = new XmlDocument();
            xml.Load(@"C:\Users\Рябов\Desktop\dts.owl");
            SituationGenerator sg = new SituationGenerator(xml);
            var situations = sg.GenerateSituationList(20, 12, 20, 20, 20);

            Assert.Fail();
        }
    }
}