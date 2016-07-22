// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectFormatter.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ObjectFormatter class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------
namespace IndiePortable.Formatter.Protocol1_1_0_0
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;


    public class ObjectFormatter
    {
        
        public static byte[] GetLengthPrefixedString(string source, Encoding encoding)
        {
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            if (string.IsNullOrEmpty(source))
            {
                return new byte[4];
            }

            var sourceBytes = encoding.GetBytes(source);
            var stringLengthBytes = BitConverter.GetBytes(sourceBytes.Length);

            var totalString = new byte[sourceBytes.Length + sizeof(int)];
            stringLengthBytes.CopyTo(totalString, 0);
            sourceBytes.CopyTo(totalString, 4);
            return totalString;
        }


        public static string GetStringFromLengthPrefixedString(byte[] source, Encoding encoding)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            if (source.Length < sizeof(int))
            {
                throw new ArgumentException($"The length of {nameof(source)} must be at least the length of an integer.", nameof(source));
            }

            var stringLength = BitConverter.ToInt32(source, 0);
            return Encoding.UTF8.GetString(source, sizeof(int), source.Length - sizeof(int));
        }


        public static string GetStringFromStream(Stream source, Encoding encoding)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            var lengthBytes = ReadFromStream(source, sizeof(int));
            var length = BitConverter.ToInt32(lengthBytes, 0);

            var stringBytes = ReadFromStream(source, length);
            var ret = encoding.GetString(stringBytes, 0, length);
            return ret;
        }


        public static byte[] ReadFromStream(Stream source, int length)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            if (length < 0)
            {
                throw new ArgumentException($"{nameof(length)} must be greater or equals 0.", nameof(length));
            }

            var target = new byte[length];
            var readLength = 0;
            while (readLength < length)
            {
                readLength += source.Read(target, readLength, length - readLength);
            }

            return target;
        }

        /// <summary>
        /// Gets the primitive value from a byte array.
        /// </summary>
        /// <param name="source">
        ///     The byte array serving as data source.
        /// </param>
        /// <param name="type">
        ///     The primitive type that shall be extracted.
        /// </param>
        /// <returns>
        ///     Returns the specified primitive value.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="type" /> does not represent a primitive type supported by the <see cref="ProtocolFormatter" />.
        /// </exception>
        public static ValueType GetPrimitiveValue(byte[] source, ObjectPrimitiveType type)
        {
            switch (type)
            {
                case ObjectPrimitiveType.Boolean: return BitConverter.ToBoolean(source, 0);
                case ObjectPrimitiveType.Int8: return (sbyte)source[0];
                case ObjectPrimitiveType.Int16: return BitConverter.ToInt16(source, 0);
                case ObjectPrimitiveType.Int32: return BitConverter.ToInt32(source, 0);
                case ObjectPrimitiveType.Int64: return BitConverter.ToInt64(source, 0);
                case ObjectPrimitiveType.UInt8: return source[0];
                case ObjectPrimitiveType.UInt16: return BitConverter.ToUInt16(source, 0);
                case ObjectPrimitiveType.UInt32: return BitConverter.ToUInt32(source, 0);
                case ObjectPrimitiveType.UInt64: return BitConverter.ToUInt64(source, 0);
                case ObjectPrimitiveType.Char: return BitConverter.ToChar(source, 0);
                case ObjectPrimitiveType.Single: return BitConverter.ToSingle(source, 0);
                case ObjectPrimitiveType.Double: return BitConverter.ToDouble(source, 0);
                default:
                    throw new ArgumentException(
                        $"The specified {nameof(ObjectPrimitiveType)} is not supported by the current {nameof(ProtocolFormatter)}.",
                        nameof(type));
            }
        }
    }
}
