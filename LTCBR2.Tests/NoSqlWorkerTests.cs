using Microsoft.VisualStudio.TestTools.UnitTesting;
using LTCBR2.Keeper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTCBR2.Tests
{
    [TestClass()]
    public class NoSqlWorkerTests
    {
        [TestMethod()]
        public void SelectTest()
        {
            var noSqlWorker = new NoSqlWorker();
            noSqlWorker.Initialization();
            var list = noSqlWorker.SelectByFilter("name","test1", "", "", "create_date", Convert.ToDateTime("11.06.2016")).GetAwaiter().GetResult();
            //var test1 = noSqlWorker.GetSituation(31485076);
            //var test2 = noSqlWorker.GetSituation(40054951);
            Assert.Fail();
        }
    }
}