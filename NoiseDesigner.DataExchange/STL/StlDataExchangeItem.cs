//-----------------------------------------------------------------------
// <copyright file="StlDataExchangeItem.cs" company="Noise Designer">
//     Copyright (c) 2013 Noise Designer. All rights reserved.
// </copyright>
// <author>Korolev Erast.</author>
//-----------------------------------------------------------------------

namespace NoiseDesigner.DataExchange.STL
{
    using System.Collections.Generic;

    /// <summary>
    /// Define of STL facet.
    /// </summary>
    public class StlDataExchangeItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StlDataExchangeItem" /> class.
        /// </summary>
        /// <param name="normal">Vector of normal.</param>
        /// <param name="triangle">List of vertex.</param>
        public StlDataExchangeItem(Normal normal, List<Vertex> triangle)
        {
            this.FacetNormal = normal;
            this.Triangle = triangle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StlDataExchangeItem" /> class.
        /// </summary>
        /// <param name="normal">Vector of normal.</param>
        /// <param name="triangle">List of vertex.</param>
        /// <param name="attributeByteCount">Byte count.</param>
        public StlDataExchangeItem(Normal normal, List<Vertex> triangle, ushort attributeByteCount)
        {
            this.FacetNormal = normal;
            this.Triangle = triangle;
            this.AttributeByteCount = attributeByteCount;
        }

        /// <summary>
        /// Gets or sets vector of normal.
        /// </summary>
        /// <value>Vector of normal.</value>
        public Normal FacetNormal { get; set; }

        /// <summary>
        /// Gets or sets vertexes of triangle.
        /// </summary>
        /// <value>List of vertex.</value>
        public List<Vertex> Triangle { get; set; }

        /// <summary>
        /// Gets or sets the attribute byte count.
        /// </summary>
        /// <value>Byte count.</value>
        public ushort AttributeByteCount { get; set; }

        /// <summary>
        /// Define of normal.
        /// </summary>
        public class Normal
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Normal" /> class.
            /// </summary>
            public Normal()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Normal" /> class.
            /// </summary>
            /// <param name="n1">First element.</param>
            /// <param name="n2">Second element.</param>
            /// <param name="n3">Third element.</param>
            public Normal(float n1, float n2, float n3)
            {
                this.N1 = n1;
                this.N2 = n2;
                this.N3 = n3;
            }

            /// <summary>
            /// Gets or sets first element of normal.
            /// </summary>
            /// <value>First element.</value>
            public float N1 { get; set; }

            /// <summary>
            /// Gets or sets second element of normal.
            /// </summary>
            /// <value>Second element.</value>
            public float N2 { get; set; }

            /// <summary>
            /// Gets or sets third element of normal.
            /// </summary>
            /// <value>Third element.</value>
            public float N3 { get; set; }
        }

        /// <summary>
        /// Define of vertex.
        /// </summary>
        public class Vertex
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Vertex" /> class.
            /// </summary>
            public Vertex()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Vertex" /> class.
            /// </summary>
            /// <param name="x">X coordinate.</param>
            /// <param name="y">Y coordinate.</param>
            /// <param name="z">Z coordinate.</param>
            public Vertex(float x, float y, float z)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
            }

            /// <summary>
            /// Gets or sets X coordinate of vertex.
            /// </summary>
            /// <value>X coordinate.</value>
            public float X { get; set; }

            /// <summary>
            /// Gets or sets Y coordinate of vertex.
            /// </summary>
            /// <value>Y coordinate.</value>
            public float Y { get; set; }

            /// <summary>
            /// Gets or sets Z coordinate of vertex.
            /// </summary>
            /// <value>Z coordinate.</value>
            public float Z { get; set; }

            /// <summary>
            /// Convert to array X, Y, Z properties.
            /// </summary>
            /// <returns>Array of points.</returns>
            public float[] ToArray()
            {
                return new[] { this.X, this.Y, this.Z };
            }
        }
    }
}
