//-----------------------------------------------------------------------
// <copyright file="Main.cs" company="Open source community.">
//     Distributed under the MIT license.
// </copyright>
// <author>Korolev Erast.</author>
//-----------------------------------------------------------------------

namespace AMP.WrapperForCLR
{
    using System.IO;
    using System.Xml.Serialization;
    using NoiseDesigner.DataExchange.Accelerators;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The main native AMP library importer. 
    /// </summary>
    public class Main
    {
        /// <summary>
        /// The native AMP library name.
        /// </summary>
        private const string AmpDll = "AMP.dll";

        #region *.Dll functions importer

        /// <summary>
        /// Import of native function for hard attribute computing.
        /// </summary>
        /// <param name="newPartiesCount">Number of new parties.</param>
        /// <param name="oldPartiesCount">Number of an existing parties</param>
        /// <param name="newParties">New parties N * 3 array. Where N - number of parties and 3 as party ID/type/purpose.</param>
        /// <param name="oldParties">An existing parties N * 3 array. Where N - number of parties and 3 as party ID/type/purpose.</param>
        /// <param name="newPartiesAttrs">New parties N1 * N2 * 2 array. Where N1 - number of new parties, N2 - max allowed number of attributes and 2 as attribute name/value.</param>
        /// <param name="oldPartiesAttrs">Old parties N1 * N2 * 2 array. Where N1 - number of old parties, N2 - max allowed number of attributes and 2 as attribute name/value.</param>
        /// <param name="newPartiesAttrsCount">Max allowed number of attributes for new parties.</param>
        /// <param name="oldPartiesAttrsCount">Max allowed number of attributes for old parties.</param>
        /// <param name="resultOfParallel">Result of operation as N1 * N2 array, where N1 - number of new parties and N2 - number of old parties.</param>
        [DllImport(AmpDll, EntryPoint = "HardAttribureComputing", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern unsafe void ImportHardAttribureComputing(
            int newPartiesCount,
            int oldPartiesCount,
            float* newParties,
            float* oldParties,
            float* newPartiesAttrs,
            float* oldPartiesAttrs,
            int newPartiesAttrsCount,
            int oldPartiesAttrsCount,
            int* resultOfParallel);

        [DllImport(AmpDll, EntryPoint = "GraphComputing", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern unsafe void ImportedGraphComputing(
            int newPartiesCount,
            int oldPartiesCount,
            int* computedAttributes,
            int* newConnections,
            int* oldConnections,
            int newConnMaxCount,
            int oldConnMaxCount,
            int* result);

        [DllImport(AmpDll, EntryPoint = "GetCurrentAcceleratorInfo", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.BStr)]
        private extern static unsafe string _getCurrentAccelerator();

        [DllImport(AmpDll, EntryPoint = "SetAccelerator", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private extern static unsafe bool _setAccelerator(string devicePath);

        [DllImport(AmpDll, EntryPoint = "GetAcceleratorsInfo", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.BStr)]
        private extern static unsafe string _getAcceleratorsInfo();
        #endregion


        public static AcceleratorsDataExchangeItem GetCurrentAccelerator()
        {
            var data = new AcceleratorsDataExchange();
            using (var reader = new StringReader(_getCurrentAccelerator()))
            {
                var serializer = new XmlSerializer(typeof(AcceleratorsDataExchange));
                data = (AcceleratorsDataExchange)serializer.Deserialize(reader);
            }

            return data.Items.FirstOrDefault();
        }

        /// <summary>
        /// Set default accelerator.
        /// </summary>
        /// <param name="devicePath">Device path to change accelerator.</param>
        /// <returns>True or false result on change.</returns>
        public static bool SetDefaultAccelerator(string devicePath)
        {
            return _setAccelerator(devicePath);
        }

        /// <summary>
        /// Get all available accelerators on computer.
        /// </summary>
        /// <returns>List of accelerators info.</returns>
        public static AcceleratorsDataExchange GetAccelerators()
        {
            var data = new AcceleratorsDataExchange();
            using (var reader = new StringReader(_getAcceleratorsInfo()))
            {
                var serializer = new XmlSerializer(typeof(AcceleratorsDataExchange));
                data = (AcceleratorsDataExchange)serializer.Deserialize(reader);
            }

            return data;
        }
    }
}
