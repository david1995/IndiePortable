// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="PrimitiveType.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the PrimitiveType enum.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    /// <summary>
    /// Provides information about the type of the primitive.
    /// </summary>
    public enum PrimitiveType
        : byte
    {
        /// <summary>
        /// A boolean value.
        /// </summary>
        Bool = 0x00,

        /// <summary>
        /// A signed byte value.
        /// </summary>
        SByte = 0x01,

        /// <summary>
        /// A 16-bit signed integer value.
        /// </summary>
        Short = 0x02,

        /// <summary>
        /// A 32-bit signed integer value.
        /// </summary>
        Int = 0x03,

        /// <summary>
        /// A 64-bit signed integer value.
        /// </summary>
        Long = 0x04,

        /// <summary>
        /// An unsigned byte value.
        /// </summary>
        Byte = 0x05,

        /// <summary>
        /// A 16-bit unsigned integer value.
        /// </summary>
        UShort = 0x06,

        /// <summary>
        /// A 32-bit unsigned integer value.
        /// </summary>
        UInt = 0x07,

        /// <summary>
        /// A 64-bit unsigned integer value.
        /// </summary>
        ULong = 0x08,

        /// <summary>
        /// An UTF-16 (CLR) or UTF-8 (binary formatted) encoded character value.
        /// </summary>
        Char = 0x09,

        /// <summary>
        /// A single-precision floating point value.
        /// </summary>
        Float = 0x0a,

        /// <summary>
        /// A double-precision floating point value.
        /// </summary>
        Double = 0x0b,

        /// <summary>
        /// An unknown or not specified language primitive.
        /// </summary>
        Unknown = 0xff
    }
}
