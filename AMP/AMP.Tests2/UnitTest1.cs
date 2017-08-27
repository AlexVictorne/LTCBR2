using System;
using AMP.WrapperForCLR;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AMP.Tests2
{
    [TestClass]
    public class UnitTest1
    {
        private float[,] newParties = new float[1000, 3];
        private float[,] oldParties = new float[800, 3];
        private float[,,] newPartiesAttrs = new float[1000, 20, 2];
        private float[,,] oldPartiesAttrs = new float[800, 30, 2];

        [TestInitialize]
        public void SetUp()
        {
            for (int i = 0; i < 1000; i++)
            {
                newParties[i, 0] = i;
                newParties[i, 1] = new Random().Next(4);
                newParties[i, 2] = new Random().Next(10);

                for (int j = 0; j < 20; j++)
                {
                    newPartiesAttrs[i, j, 0] = new Random().Next(4);
                    newPartiesAttrs[i, j, 1] = new Random().Next(1, 3);
                }
            }

            for (int i = 0; i < 800; i++)
            {
                oldParties[i, 0] = i;
                oldParties[i, 1] = new Random().Next(4);
                oldParties[i, 2] = new Random().Next(10);

                for (int j = 0; j < 30; j++)
                {
                    oldPartiesAttrs[i, j, 0] = new Random().Next(4);
                    oldPartiesAttrs[i, j, 1] = new Random().Next(1, 3);
                }
            }
        }

        [TestMethod]
        public unsafe void TestAttributesComputing()
        {
            var result = new int[1000, 800];
            fixed (float* arr1 = &newParties[0, 0], arr2 = &oldParties[0, 0], arr3 = &newPartiesAttrs[0, 0, 0], arr4 = &oldPartiesAttrs[0, 0, 0])
            {
                fixed (int* arr5 = &result[0, 0])
                {
                    Main.ImportHardAttribureComputing(1000, 800, arr1, arr2, arr3, arr4, 20, 30, arr5);
                }
            }

            var successCount = 0;
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 800; j++)
                {
                    successCount += result[i, j];
                }
            }

            Assert.IsTrue(successCount > 0, "Success count equal 0.");
        }
    }
}
