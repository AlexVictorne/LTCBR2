//-----------------------------------------------------------------------
// <copyright file="AcceleratorsDataExchangeItem.cs" company="Noise Designer">
//     Copyright (c) 2013 Noise Designer. All rights reserved.
// </copyright>
// <author>Korolev Erast.</author>
//-----------------------------------------------------------------------

namespace NoiseDesigner.DataExchange.Accelerators
{
    using System.Xml.Serialization;

    /// <summary>
    /// Define of accelerator info item.
    /// </summary>
    public class AcceleratorsDataExchangeItem
    {
        /// <summary>
        /// Accelerator device path.
        /// </summary>
        private string devicePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="AcceleratorsDataExchangeItem" /> class.
        /// </summary>
        public AcceleratorsDataExchangeItem()
        {
        }

        /// <summary>
        /// Gets or sets accelerator description.
        /// </summary>
        /// <value>Description of accelerator.</value>
        [XmlAttribute("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets accelerator path.
        /// </summary>
        /// <value>Device path.</value>
        [XmlAttribute("devicepath")]
        public string DevicePath
        {
            get { return this.devicePath.Replace(";", "&"); }
            set { this.devicePath = value.Replace("&", ";"); }
        }

        /// <summary>
        /// Gets or sets a value.
        /// </summary>
        /// <value>Supports CPU shared memory.</value>
        [XmlAttribute("supportscpusharedmemory")]
        public bool SupportsCpuSharedMemory { get; set; }

        /// <summary>
        /// Gets or sets supports double precision.
        /// </summary>
        /// <value>Supports double precision.</value>
        [XmlAttribute("supportsdoubleprecision")]
        public bool SupportsDoublePrecision { get; set; }

        /// <summary>
        /// Gets or sets supports limited double precision.
        /// </summary>
        /// <value>Supports limited double precision</value>
        [XmlAttribute("supportslimiteddoubleprecision")]
        public bool SupportsLimitedDoublePrecision { get; set; }

        /// <summary>
        /// Gets or sets connected with display or not.
        /// </summary>
        /// <value>Connected with display.</value>
        [XmlAttribute("hasdisplay")]
        public bool HasDisplay { get; set; }

        /// <summary>
        /// Gets or sets supports debug.
        /// </summary>
        /// <value>Supports debug.</value>
        [XmlAttribute("isdebug")]
        public bool IsDebug { get; set; }

        /// <summary>
        /// Gets or sets is emulate device or not.
        /// </summary>
        /// <value>Is emulate device.</value>
        [XmlAttribute("isemulated")]
        public bool IsEmulated { get; set; }

        /// <summary>
        /// Gets or sets dedicated memory of device.
        /// </summary>
        /// <value>Dedicated memory of device.</value>
        [XmlAttribute("dedicatedmemory")]
        public long DedicatedMemory { get; set; }

        /// <summary>
        /// Gets or sets version of device.
        /// </summary>
        /// <value>Version of device.</value>
        [XmlAttribute("version")]
        public long Version { get; set; }
    }
}
