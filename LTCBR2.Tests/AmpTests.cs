// HIDE AMP using AMP.WrapperForCLR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LTCBR2.Tests
{
    [TestClass]
    public class AmpTests
    {
        const int cycleCount = 100;
        const int newCount = 100;
        const int oldCount = 100;
        const int newAttr = 20;
        const int oldAttr = 20;

        private int[,] newParties = new int[newCount, 3];
        private int[,] oldParties = new int[oldCount, 3];
        private int[,,] newPartiesAttrs = new int[newCount, newAttr, 2];
        private int[,,] oldPartiesAttrs = new int[oldCount, oldAttr, 2];

        [TestInitialize]
        public void SetUp()
        {
            for (int i = 0; i < newCount; i++)
            {
                newParties[i, 0] = i;
                newParties[i, 1] = new Random().Next(4);
                newParties[i, 2] = new Random().Next(10);

                for (int j = 0; j < newAttr; j++)
                {
                    newPartiesAttrs[i, j, 0] = new Random().Next(4);
                    newPartiesAttrs[i, j, 1] = new Random().Next(1, 3);
                }
            }

            for (int i = 0; i < oldCount; i++)
            {
                oldParties[i, 0] = i;
                oldParties[i, 1] = new Random().Next(4);
                oldParties[i, 2] = new Random().Next(10);

                for (int j = 0; j < oldAttr; j++)
                {
                    oldPartiesAttrs[i, j, 0] =  new Random().Next(4);
                    oldPartiesAttrs[i, j, 1] = new Random().Next(1, 3);
                }
            }
        }

        [TestMethod]
        public unsafe void TestAttributesComputing1()
        {
            var a = 0;// Main.GetAccelerators().Items[0];
            var b = 0;// Main.SetDefaultAccelerator(a.DevicePath);
            var result = new int[newCount, oldCount];
            for (int k = 0; k < cycleCount; k++)
            {
                fixed (int* arr1 = &newParties[0, 0], arr2 = &oldParties[0, 0], arr3 = &newPartiesAttrs[0, 0, 0], arr4 = &oldPartiesAttrs[0, 0, 0])
                {
                    fixed (int* arr5 = &result[0, 0])
                    {
                        //Main.ImportHardAttribureComputing(newCount, oldCount, arr1, arr2, arr3, arr4, newAttr, oldAttr, arr5);
                    }
                }

                var successCount = 0;
                for (int i = 0; i < newCount; i++)
                {
                    for (int j = 0; j < oldCount; j++)
                    {
                        successCount += result[i, j];
                    }
                }
            }
            //Assert.IsTrue(successCount > 0, "Success count equal 0.");
        }
        [TestMethod]
        public unsafe void TestAttributesComputing2()
        {
            var a = 0;// Main.GetAccelerators().Items[1];
            var b = 0;//Main.SetDefaultAccelerator(a.DevicePath);
            var result = new int[newCount, oldCount];
            for (int k = 0; k < cycleCount; k++)
            {
                fixed (int* arr1 = &newParties[0, 0], arr2 = &oldParties[0, 0], arr3 = &newPartiesAttrs[0, 0, 0], arr4 = &oldPartiesAttrs[0, 0, 0])
                {
                    fixed (int* arr5 = &result[0, 0])
                    {
                        //Main.ImportHardAttribureComputing(newCount, oldCount, arr1, arr2, arr3, arr4, newAttr, oldAttr, arr5);
                    }
                }

                var successCount = 0;
                for (int i = 0; i < newCount; i++)
                {
                    for (int j = 0; j < oldCount; j++)
                    {
                        successCount += result[i, j];
                    }
                }
            }
            //Assert.IsTrue(successCount > 0, "Success count equal 0.");
        }
        [TestMethod]
        public unsafe void TestAttributesComputing3()
        {
            var a = 0;// Main.GetAccelerators().Items[2];
            var b = 0;// Main.SetDefaultAccelerator(a.DevicePath);
            var result = new int[newCount, oldCount];
            for (int k = 0; k < cycleCount; k++)
            {
                fixed (int* arr1 = &newParties[0, 0], arr2 = &oldParties[0, 0], arr3 = &newPartiesAttrs[0, 0, 0], arr4 = &oldPartiesAttrs[0, 0, 0])
                {
                    fixed (int* arr5 = &result[0, 0])
                    {
                        //Main.ImportHardAttribureComputing(newCount, oldCount, arr1, arr2, arr3, arr4, newAttr, oldAttr, arr5);
                    }
                }

                var successCount = 0;
                for (int i = 0; i < newCount; i++)
                {
                    for (int j = 0; j < oldCount; j++)
                    {
                        successCount += result[i, j];
                    }
                }
            }
            //Assert.IsTrue(successCount > 0, "Success count equal 0.");
      }

        [TestMethod]
        public unsafe void TestAttributesComputing4()
        {
            var a = 0;// Main.GetAccelerators().Items[3];
            var b = 0;// Main.SetDefaultAccelerator(a.DevicePath);
            var result = new int[newCount, oldCount];
            for (int k = 0; k < cycleCount; k++)
            {
                fixed (int* arr1 = &newParties[0, 0], arr2 = &oldParties[0, 0], arr3 = &newPartiesAttrs[0, 0, 0], arr4 = &oldPartiesAttrs[0, 0, 0])
                {
                    fixed (int* arr5 = &result[0, 0])
                    {
                        //Main.ImportHardAttribureComputing(newCount, oldCount, arr1, arr2, arr3, arr4, newAttr, oldAttr, arr5);
                    }
                }

                var successCount = 0;
                for (int i = 0; i < newCount; i++)
                {
                    for (int j = 0; j < oldCount; j++)
                    {
                        successCount += result[i, j];
                    }
                }
            }
            //Assert.IsTrue(successCount > 0, "Success count equal 0.");
        }

        [TestMethod]
        public void TestTest()
        {
            //TestAttributesComputing1();
            TestAttributesComputing2();
            TestAttributesComputing3();
            Assert.IsTrue(true);
        }
    }
}
