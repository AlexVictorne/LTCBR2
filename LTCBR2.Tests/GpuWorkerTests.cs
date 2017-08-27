using Microsoft.VisualStudio.TestTools.UnitTesting;
using LTCBR2.Searcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTCBR2.Tests
{
    [TestClass()]
    public class GpuWorkerTests
    {
        [TestMethod()]
        public void GetListOfDevicesTest()
        {
            GpuWorker gw = new GpuWorker();
            gw.GetListOfDevices();
            Assert.Fail();
        }
    }
}
