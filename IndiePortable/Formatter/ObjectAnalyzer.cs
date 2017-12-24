// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectAnalyzer.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the static ObjectAnalyzer class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;
    using System.Reflection;
    using Protocol1_0_0_0;

    /// <summary>
    /// Provides methods for analyzing objects.
    /// </summary>
    public static class ObjectAnalyzer
    {
        /// <summary>
        /// Gets the type of an object.
        /// </summary>
        /// <param name="value">
        ///     The object that shall be analyzed.
        /// </param>
        /// <returns>
        ///     Returns the byte representation of the object type.
        /// </returns>
        public static byte[] GetObjectType(object value)
        { 
            if (value == null)
            {
                return new byte[4];
            }

            var type = value.GetType();
            var typeInfo = type.GetTypeInfo();

            return value == null // null
                   ? new byte[4]
                   : type == typeof(string) // string
                   ? new byte[] { (byte)ObjectType.String, 0x00, 0x00, 0x00 }
                   : typeInfo.IsPrimitive // language primitive
                   ? new byte[] { (byte)ObjectType.Primitive, (byte)GetPrimitiveType(value.GetType()), 0x00, 0x00 }
                   : typeInfo.IsEnum // enum
                   ? new byte[] { (byte)ObjectType.Enum, (byte)GetPrimitiveType(Enum.GetUnderlyingType(value.GetType())), 0x00, 0x00 }
                   : typeInfo.IsArray // array
                   ? new byte[] { (byte)ObjectType.Array, 0x00, 0x00, 0x00 }
                   : new byte[] { (byte)ObjectType.Object, 0x00, 0x00, 0x00 }; // object
        }

        /// <summary>
        /// Gets the <see cref="ObjectType" /> for a specified value.
        /// </summary>
        /// <param name="value">
        ///     The value that shall be analyzed.
        /// </param>
        /// <returns>
        ///     Returns the <see cref="ObjectType" /> for the value.
        /// </returns>
        public static ObjectType GetPrimaryObjectType(object value)
        {
            if (value == null)
            {
                return ObjectType.Null;
            }

            var type = value.GetType();
            var typeInfo = type.GetTypeInfo();

            return type == typeof(string) // string
                 ? ObjectType.String
                 : typeInfo.IsPrimitive // language primitive
                 ? ObjectType.Primitive
                 : typeInfo.IsEnum // enum type
                 ? ObjectType.Enum
                 : typeInfo.IsArray // array type
                 ? ObjectType.Array
                 : ObjectType.Object;
        }
            

        /// <summary>
        /// Gets the <see cref="PrimitiveType" /> of a primitive.
        /// </summary>
        /// <param name="type">
        ///     The <see cref="Type" /> that shall be analyzed.
        /// </param>
        /// <returns>
        ///     Returns the <see cref="PrimitiveType" /> for <paramref name="type" />.
        /// </returns>
        public static PrimitiveType GetPrimitiveType(Type type)
            => type == typeof(bool)
             ? PrimitiveType.Bool
             : type == typeof(sbyte)
             ? PrimitiveType.SByte
             : type == typeof(short)
             ? PrimitiveType.Short
             : type == typeof(int)
             ? PrimitiveType.Int
             : type == typeof(long)
             ? PrimitiveType.Long
             : type == typeof(byte)
             ? PrimitiveType.Byte
             : type == typeof(ushort)
             ? PrimitiveType.UShort
             : type == typeof(uint)
             ? PrimitiveType.UInt
             : type == typeof(ulong)
             ? PrimitiveType.ULong
             : type == typeof(char)
             ? PrimitiveType.Char
             : type == typeof(float)
             ? PrimitiveType.Float
             : type == typeof(double)
             ? PrimitiveType.Double
             : PrimitiveType.Unknown;
    }
}
