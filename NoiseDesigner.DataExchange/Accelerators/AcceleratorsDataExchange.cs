//-----------------------------------------------------------------------
// <copyright file="AcceleratorsDataExchange.cs" company="Noise Designer">
//     Copyright (c) 2013 Noise Designer. All rights reserved.
// </copyright>
// <author>Korolev Erast.</author>
//-----------------------------------------------------------------------

namespace NoiseDesigner.DataExchange.Accelerators
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Define of accelerators info.
    /// </summary>
    public class AcceleratorsDataExchange
    {
        /// <summary>
        /// Gets or sets accelerators info.
        /// </summary>
        /// <value>List of accelerators.</value>
        [XmlElement("Item")]
        public List<AcceleratorsDataExchangeItem> Items { get; set; }
    }
}
