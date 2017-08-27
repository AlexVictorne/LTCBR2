using Microsoft.VisualStudio.TestTools.UnitTesting;
using LTCBR2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LTCBR2.Keeper;
using LTCBR2.Types;

namespace LTCBR2.Utils.Tests
{
    [TestClass()]
    public class ImportFromConstructorTests
    {
        [TestMethod()]
        public void load_ppsTest()
        {
            ImportFromConstructor ifc = new ImportFromConstructor();
            XmlDocument xml = new XmlDocument();
            xml.Load(@"C:\Users\Рябов\Desktop\situationfromgraphicconstructor (2).xml");
            var newSit = ifc.load_pps(xml.InnerXml);
            newSit.create_date = DateTime.Today;
            //newSit.coordinates = new List<Coordinate>();
            //var gw = new GraphWorker();
            //newSit = gw.FillCoordinates(newSit, gw.MakeGraph(gw.SituationToGraph(newSit.participants)));
            //var noSqlWorker = new NoSqlWorker();
            //noSqlWorker.Initialization();
            //noSqlWorker.Insert(newSit);
        }
        
    }
}