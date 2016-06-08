using System;
using LTCBR2Gpu;

namespace LTCBR2.Searcher
{
    public class GpuWorker
    {
        public GpuWorker()
        {
            var gpu = new GpuWork();
            var myString = gpu.GetPlatforms();
            gpu.SetPlatform(0);
            var myString2 = gpu.GetDevices();
            gpu.SetDevice(1);
            var local = gpu.Initialization();
        }
    
        

    }
}