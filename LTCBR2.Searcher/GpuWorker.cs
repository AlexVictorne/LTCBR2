using System;
using System.Collections.Generic;
using System.Linq;
using AMP.WrapperForCLR;
using LTCBR2Gpu;

namespace LTCBR2.Searcher
{
    public class GpuWorker
    {
        public GpuWorker()
        {
            //var gpu = new GpuWork();
            //var myString = gpu.GetPlatforms();
            //gpu.SetPlatform(0);
            //var myString2 = gpu.GetDevices();
            //gpu.SetDevice(1);
            //var local = gpu.Initialization();
        }

        class DeviceInfo
        {
            public string DeviceName { get; set; }
            public string DevicePath { get; set; }
        }

        public List<string> GetListOfDevices()
        {
             var devicesInfo = Main.GetAccelerators().Items;
            return devicesInfo.Select(info => info.Description).ToList();
        }

        public void SetDevice(string selected)
        {
            var selectedDevice = Main.GetAccelerators().Items.First(x => x.Description == selected).DevicePath;
        }
    
        

    }
}