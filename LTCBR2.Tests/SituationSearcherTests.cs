using Microsoft.VisualStudio.TestTools.UnitTesting;
using LTCBR2.Searcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LTCBR2.Keeper;
using LTCBR2.Utils;

namespace LTCBR2.Tests
{
    [TestClass()]
    public class SituationSearcherTests
    {
        [TestMethod()]
        public void RoughSearchTest()
        {
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            var test1 = noSqlWorker.GetSituation(31485076);
            var test2 = noSqlWorker.GetSituation(40054951);
            var testGpu1 = Tools.SituationToSituationGpu(test1);
            var testGpu2 = Tools.SituationToSituationGpu(test2);
            SituationSearcher ss = new SituationSearcher();
            int[,] resultCompared;
            double roughMark;
            double roughLimit = 100;
            ss.
            ss.RoughSearch(testGpu1.Participants, testGpu1.Attributes, testGpu2.Participants, testGpu2.Attributes,
                roughLimit, out roughMark, out resultCompared);
            
        }

        [TestMethod()]
        public void AccurateSearchTest()
        {
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            var test1 = noSqlWorker.GetSituation(28805302);
            var test2 = noSqlWorker.GetSituation(62476613);
            var testGpu1 = Tools.SituationToSituationGpu(test1);
            var testGpu2 = Tools.SituationToSituationGpu(test2);
            SituationSearcher ss = new SituationSearcher();
            int[,] resultCompared;
            int[,] accurateCompared;
            double roughMark;
            double roughLimit = 100;
            ss.RoughSearch(testGpu1.Participants, testGpu1.Attributes, testGpu2.Participants, testGpu2.Attributes,
                roughLimit, out roughMark, out resultCompared);
            //ss.AccurateSearch(testGpu1.Connections,testGpu2.Connections,resultCompared,out accurateCompared);
            

        }
    }
}