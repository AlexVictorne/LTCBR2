#include "stdafx.h"
#include "CppUnitTest.h"
#include "../LTCBR2.Gpu/LTCBR2.Gpu.h"
#pragma comment(lib, "../Debug/LTCBR2.Gpu.lib")


using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace LTCBR2Gpu;


namespace LTCBR2GpuTests
{		
	TEST_CLASS(UnitTest1)
	{
	public:
		
		TEST_METHOD(TestMethod1)
		{
			
			// TODO: Your test code here
			Gpu gpu;
			auto vect = gpu.GetPlatforms();
			for (int i=0;i<vect.size();i++)
			{
				auto info = vect.at(i);
			}
			auto vect2 = gpu.GetDevices();
			for (int i=0;i<vect2.size();i++)
			{
				auto info = vect2.at(i);
			}
		}

	};
}