// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectType.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ObjectType enum.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Protocol1_0_0_0
{
    /// <summary>
    /// Provides information about the serialization type of an object.
    /// </summary>
    public enum ObjectType
        : byte
    {
        /// <summary>
        /// The <c>null</c> (or undefined) type.
        /// </summary>
        Null = 0x00,

        /// <summary>
        /// The object type.
        /// </summary>
        Object = 0x01,

        /// <summary>
        /// The string type.
        /// </summary>
        String = 0x02,

        /// <summary>
        /// The primitive type.
        /// </summary>
        Primitive = 0x03,

        /// <summary>
        /// The Enum type.
        /// </summary>
        Enum = 0x04,

        /// <summary>
        /// The array type.
        /// </summary>
        Array = 0x05
    }
}
