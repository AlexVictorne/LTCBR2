using Microsoft.VisualStudio.TestTools.UnitTesting;
using LTCBR2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LTCBR2.Keeper;

namespace LTCBR2.Utils.Tests
{
    [TestClass()]
    public class GraphWorkerTests
    {
        [TestMethod()]
        public void MakeGraphGraphMakeTest()
        {
            GraphWorker gw = new GraphWorker();
            //var i = gw.MakeGraph();
            Assert.Fail();
        }

        [TestMethod()]
        public void ToGraphTest()
        {
            //load situation
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            var selectedSituation = noSqlWorker.GetSituation(13500410);
            //graph
            var gw = new GraphWorker();
            var output = gw.SituationToGraph(selectedSituation.participants);
        }

        [TestMethod()]
        public void ToGraphWithCoordinatesTest()
        {
            //load situation
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            var selectedSituation = noSqlWorker.GetSituation(13500410);
            //graph
            var gw = new GraphWorker();
            var output = gw.SituationToGraph(selectedSituation.participants);
            var outputWithCoordinates = gw.MakeGraph(output);
        }

        [TestMethod()]
        public void ToCoordinateFromStringGraphTest()
        {
            //load situation
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            var selectedSituation = noSqlWorker.GetSituation(13500410);
            //graph
            var gw = new GraphWorker();
            var output = gw.SituationToGraph(selectedSituation.participants);
            var outputWithCoordinates = gw.MakeGraph(output);
            var outputCoordinates = gw.FillCoordinates(selectedSituation, outputWithCoordinates);
        }
    }
}