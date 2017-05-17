// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectPrimitiveType.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ObjectPrimitiveType enum.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Protocol1_1_0_0
{

    public enum ObjectPrimitiveType
        : byte
    {
        /// <summary>
        /// The represented type is not a primitive type.
        /// </summary>
        NonPrimitive = 0x00,

        /// <summary>
        /// The boolean type, also known as <see cref="bool" />.
        /// </summary>
        Boolean = 0x01,

        /// <summary>
        /// The character type, also known as <see cref="char" />.
        /// </summary>
        Char = 0x02,

        /// <summary>
        /// The 8-bit signed integer, also known as signed byte or <see cref="sbyte" />.
        /// </summary>
        Int8 = 0x03,

        /// <summary>
        /// The 16-bit signed integer, also known as <see cref="short" />.
        /// </summary>
        Int16 = 0x04,

        /// <summary>
        /// The 32-bit signed integer, also known as <see cref="int" />.
        /// </summary>
        Int32 = 0x05,

        /// <summary>
        /// The 64-bit signed integer, also known as <see cref="long" />.
        /// </summary>
        Int64 = 0x06,

        /// <summary>
        /// The 8-bit unsigned integer, also known as <see cref="byte" />.
        /// </summary>
        UInt8 = 0x07,

        /// <summary>
        /// The 16-bit unsigned integer, also known as <see cref="ushort" />.
        /// </summary>
        UInt16 = 0x08,

        /// <summary>
        /// The 32-bit unsigned integer, also known as <see cref="uint" />.
        /// </summary>
        UInt32 = 0x09,

        /// <summary>
        /// The 64-bit unsigned integer, also known as <see cref="ulong" />.
        /// </summary>
        UInt64 = 0x0a,

        /// <summary>
        /// The single-precision floating point number type, also known as <see cref="float" />.
        /// </summary>
        Single = 0x0b,

        /// <summary>
        /// The double-precision floating point number type, also known as <see cref="double" />.
        /// </summary>
        Double = 0x0c,

        /// <summary>
        /// The decimal number type, also known as <see cref="decimal" />.
        /// </summary>
        Decimal = 0x0d
    }
}
