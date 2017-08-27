namespace AMP.Tests
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AMP.WrapperForCLR;
    using NoiseDesigner.DataExchange.Accelerators;
    using System.Threading;

    [TestClass]
    public class AmpTest
    {
        private float[,] newParties = new float[10000, 3];
        private float[,] oldParties = new float[2000, 3];
        private float[,,] newPartiesAttrs = new float[10000, 20, 2];
        private float[,,] oldPartiesAttrs = new float[2000, 30, 2];

        private int[,] realAttributes;
        private int[,] realNewConnections;
        private int[,] realOldConnections;

        [TestInitialize]
        public void SetUp()
        {
            realAttributes = new int[5, 4]
            {
                { 0, 1, 0, 1 },
                { 0, 1, 0, 0 },
                { 1, 0, 1, 0 },
                { 0, 1, 0, 1 },
                { 0, 0, 1, 0 }
            };

            realNewConnections = new int[5, 3]
            {
                { 1, 2, -1 },
                { 0, 3, 4 },
                { 0, 3, -1 },
                { 1, 2, -1 },
                { 2, 3, -1 }
            };

            realOldConnections = new int[4, 2]
            {
                { 1, 2 },
                { 0, 3 },
                { 0, 3 },
                { 1, 2 }
            };

            for (int i = 0; i < 10000; i++)
            {
                newParties[i, 0] = i;
                newParties[i, 1] = 1;//new Random().Next(4);
                newParties[i, 2] = 2;//new Random().Next(10);

                for (int j = 0; j < 20; j++)
                {
                    newPartiesAttrs[i, j, 0] = 3;//new Random().Next(4);
                    newPartiesAttrs[i, j, 1] = 4; //new Random().Next(1, 3);
                }
            }

            for (int i = 0; i < 2000; i++)
            {
                oldParties[i, 0] = i;
                oldParties[i, 1] = 1;//new Random().Next(4);
                oldParties[i, 2] = 2;//new Random().Next(10);

                for (int j = 0; j < 30; j++)
                {
                    oldPartiesAttrs[i, j, 0] = 3;// new Random().Next(4);
                    oldPartiesAttrs[i, j, 1] = 4;//new Random().Next(1, 3);
                }
            }
        }

        [TestMethod]
        public unsafe void test1()
        {
            var resultOfParallel = new int[5, 4];
            for (int u = 0; u < 5; u++)
            {
                for (int i = 0; i < 4; i++)
                {
                    int newConnsCount = 0;
                    int equalConnsCount = 0;
                    if (realAttributes[u, i] == 1)
                    {
                        //this connections
                        for (int j = 0; j < 3; j++)
                        {
                            int newConnection = realNewConnections[u, j];
                            if (newConnection != -1)
                            {
                                newConnsCount++;
                                bool resultSubConn = false;
                                //check child with old
                                for (int v = 0; v < 4; v++)
                                {
                                    if (realAttributes[newConnection, v] == 1)
                                    {
                                        for (int m = 0; m < 2; m++)
                                        {
                                            if (realOldConnections[v, m] == i)
                                            {
                                                resultSubConn = true;
                                                equalConnsCount++;
                                                break;
                                            }
                                        }

                                        if (resultSubConn)
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (!resultSubConn)
                                {
                                    resultOfParallel[u, j] = -1;
                                }
                            }
                            else
                            {
                                if (newConnsCount == equalConnsCount)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public unsafe void GraphComputing()
        {
            var result = new int[5, 3];
            fixed (int* arr1 = &realNewConnections[0, 0], arr2 = &realOldConnections[0, 0], arr3 = &realAttributes[0, 0], arr4 = &result[0, 0])
            {
                Main.ImportedGraphComputing(5, 4, arr3, arr1, arr2, 3, 2, arr4);
            }

            Assert.IsTrue(result != realNewConnections);
        }

        [TestMethod]
        public unsafe void TestAttributesComputing1()
        {
            var msec = DateTime.Now;
            //var a = Main.GetAccelerators().Items;
            var result = new int[10000, 2000];
            fixed (float* arr1 = &newParties[0, 0], arr2 = &oldParties[0, 0], arr3 = &newPartiesAttrs[0, 0, 0], arr4 = &oldPartiesAttrs[0, 0, 0])
            {
                fixed (int* arr5 = &result[0, 0])
                {
                    Main.ImportHardAttribureComputing(10000, 2000, arr1, arr2, arr3, arr4, 20, 30, arr5);
                }
            }

            var successCount = 0;
            for (int i = 0; i < 10000; i++)
            {
                for (int j = 0; j < 2000; j++)
                {
                    successCount += result[i, j];
                }
            }

            var tempResultTime = (msec - DateTime.Now);
            var resultTime1 = (int)(tempResultTime.Seconds * 1000 + tempResultTime.Milliseconds);
            Assert.IsTrue(successCount > 0, "Success count equal 0.");
        }

        //[TestMethod]
        public unsafe void TestAttributesComputing2()
        {
            var a = Main.GetAccelerators().Items;
            Main.SetDefaultAccelerator(a[3].DevicePath);

            var msec = DateTime.Now;
            var result = new int[10000, 2000];
            fixed (float* arr1 = &newParties[0, 0], arr2 = &oldParties[0, 0], arr3 = &newPartiesAttrs[0, 0, 0], arr4 = &oldPartiesAttrs[0, 0, 0])
            {
                fixed (int* arr5 = &result[0, 0])
                {
                    Main.ImportHardAttribureComputing(10000, 2000, arr1, arr2, arr3, arr4, 20, 30, arr5);
                }
            }

            var successCount = 0;
            for (int i = 0; i < 10000; i++)
            {
                for (int j = 0; j < 2000; j++)
                {
                    successCount += result[i, j];
                }
            }

            var tempResultTime = (msec - DateTime.Now);
            var resultTime2 = (int)(tempResultTime.Seconds * 1000 + tempResultTime.Milliseconds);
            Assert.IsTrue(successCount > 0, "Success count equal 0.");
        }

        [TestMethod]
        public unsafe void TestAttributesComputing3()
        {
            var qwe = "";
        }
    }
}
