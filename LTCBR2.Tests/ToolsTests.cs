using Microsoft.VisualStudio.TestTools.UnitTesting;
using LTCBR2.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LTCBR2.Keeper;
using LTCBR2.Types;


namespace LTCBR2.Tests
{

    [TestClass()]
    public class ToolsTests
    {
        [TestMethod()]
        public void SituationToXmlTest()
        {
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            var selectedSituation = noSqlWorker.GetSituation(30544706);
            var ser = new XmlSerializer(typeof(Situation));
            var fullPath = Path.Combine("G://Situation" + ".xml");
            var writer = new FileStream(fullPath, FileMode.Create);
            ser.Serialize(writer, selectedSituation);
            writer.Close();
        }
        [TestMethod()]
        public void SituationToSituationGpuTest()
        {
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            var selectedSituation = noSqlWorker.GetSituation(31485076);
            var result = Tools.SituationToSituationGpu(selectedSituation);
        }

        [TestMethod()]
        public void ConvertTest()
        {
            string s = "0.2";
            var tmp1 = Tools.ConvertToInt(s);
            var tmp2 = Tools.ConvertToInt(s);
            Assert.AreEqual(tmp1,tmp2);
        }

        [TestMethod()]
        public void SituationValidateTest()
        {
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            var selectedSituation = noSqlWorker.GetSituation(12036987);
            var newSituation = Tools.ValidateSituation(selectedSituation);
        }
       
        //[TestMethod()]
        //public void ConvertToStringTest()
        //{
        //    Assert.Fail();
        //}
    }
}