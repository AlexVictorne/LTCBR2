//-----------------------------------------------------------------------
// <copyright file="StlDataExchange.cs" company="Noise Designer">
//     Copyright (c) 2013 Noise Designer. All rights reserved.
// </copyright>
// <author>Korolev Erast.</author>
//-----------------------------------------------------------------------

namespace NoiseDesigner.DataExchange.STL
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Define of STL format.
    /// </summary>
    public class StlDataExchange
    {
        /// <summary>
        /// Gets or sets STL model description.
        /// </summary>
        /// <value>Description of file.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets number of triangles in STL file.
        /// </summary>
        /// <value>Number of triangles.</value>
        public uint NumberOfTriangles { get; set; }

        /// <summary>
        /// Gets or sets triangles specification.
        /// </summary>
        /// <value>List of triangles.</value>
        public List<StlDataExchangeItem> Items { get; set; } 
    }
}
