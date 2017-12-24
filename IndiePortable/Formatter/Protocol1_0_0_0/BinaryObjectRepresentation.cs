// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryObjectRepresentation.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the BinaryObjectRepresentation class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Protocol1_0_0_0
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an object in its binary values. This class is only intended for internal use.
    /// </summary>
    internal class BinaryObjectRepresentation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryObjectRepresentation" /> class.
        /// </summary>
        /// <param name="objectId">
        ///     The object ID of the represented object in the graph.
        /// </param>
        /// <param name="fullType">
        ///     The full binary value type.
        /// </param>
        /// <param name="clrType">
        ///     The assembly qualified name of the CLR type.
        /// </param>
        /// <param name="objectBody">
        ///     The binary body of the object.
        /// </param>
        public BinaryObjectRepresentation(int objectId, byte[] fullType, string clrType, byte[] objectBody)
        {
            this.ObjectID = objectId;
            this.Type = (ObjectType)fullType[0];
            this.FullType = fullType;
            this.ClrType = clrType;
            this.ObjectBody = objectBody;
        }

        /// <summary>
        /// Gets the object ID of the represented <see cref="object" /> in the current graph.
        /// </summary>
        /// <value>
        ///     Contains the object ID of the represented <see cref="object" /> in the current graph.
        /// </value>
        internal int ObjectID { get; private set; }

        /// <summary>
        /// Gets the <see cref="ObjectType" /> of the represented <see cref="object" />.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="ObjectType" /> of the represented <see cref="object" />.
        /// </value>
        internal ObjectType Type { get; private set; }

        /// <summary>
        /// Gets the full binary value type.
        /// </summary>
        /// <value>
        ///     Contains the full binary value type.
        /// </value>
        internal byte[] FullType { get; private set; }

        /// <summary>
        /// Gets the assembly qualified name of the CLR type.
        /// </summary>
        /// <value>
        ///     Contains the assembly qualified name of the CLR type.
        /// </value>
        internal string ClrType { get; private set; }

        /// <summary>
        /// Gets the binary body of the represented <see cref="object" />.
        /// </summary>
        /// <value>
        ///     Contains the binary body of the represented <see cref="object" />.
        /// </value>
        internal byte[] ObjectBody { get; private set; }

        /// <summary>
        /// Gets the properties of the <see cref="BinaryObjectRepresentation" />.
        /// </summary>
        /// <value>
        ///     Contains the properties of the <see cref="BinaryObjectRepresentation" />.
        /// </value>
        internal Dictionary<Tuple<string, byte[]>, byte[]> Properties { get; } = new Dictionary<Tuple<string, byte[]>, byte[]>();

        /// <summary>
        /// Gets or sets the represented <see cref="object" />,
        /// or <c>null</c> if the <see cref="object" /> has not been fully de-serialized.
        /// </summary>
        /// <value>
        ///     Contains the represented <see cref="object" />.
        /// </value>
        internal object Result { get; set; }
    }
}
