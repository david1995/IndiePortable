// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteObjectHeader.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ByteObjectHeader class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Protocol1_1_0_0
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    public class ByteObjectHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteObjectHeader" /> class.
        /// </summary>
        /// <param name="objectId">
        ///     The object identifier.
        /// </param>
        /// <param name="majorType">
        ///     The major type of the represented object.
        /// </param>
        /// <param name="primitiveType">
        ///     The primitive type of the represented object.
        /// </param>
        /// <param name="clrTypeName">
        ///     Name of the color type.
        /// </param>
        /// <param name="length">
        ///     The length of the byte object body.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="clrTypeName" /> is <c>null</c>.</para>
        /// </exception>
        protected ByteObjectHeader(int objectId, ObjectType majorType, ObjectPrimitiveType primitiveType, string clrTypeName, int length)
        {
            if (clrTypeName == null)
            {
                throw new ArgumentNullException(nameof(clrTypeName));
            }

            this.ObjectId = objectId;
            this.MajorType = majorType;
            this.ClrTypeName = clrTypeName;
            this.Length = length;
        }


        public int ObjectId { get; private set; }


        public ObjectType MajorType { get; private set; }


        public ObjectPrimitiveType PrimitiveType { get; private set; }


        public string ClrTypeName { get; private set; }


        public int Length { get; private set; }


        public static ByteObjectHeader FromStream(Stream source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.CanRead)
            {
                throw new ArgumentException("The specified stream can not be read.", nameof(source));
            }

            // object Id
            var objectIdBytes = ObjectFormatter.ReadFromStream(source, sizeof(int));
            var objectId = BitConverter.ToInt32(objectIdBytes, 0);

            // object type
            var objectTypeBytes = ObjectFormatter.ReadFromStream(source, sizeof(byte) * 4);
            var objectMajorType = (ObjectType)objectTypeBytes[0];
            var objectPrimitiveType = (ObjectPrimitiveType)objectTypeBytes[1];
            
            // clr type name
            var clrTypeName = ObjectFormatter.GetStringFromStream(source, Encoding.UTF8);

            // object length
            var objectLengthBytes = ObjectFormatter.ReadFromStream(source, sizeof(int));
            var objectLength = BitConverter.ToInt32(objectLengthBytes, 0);

            return new ByteObjectHeader(objectId, objectMajorType, objectPrimitiveType, clrTypeName, objectLength);
        }

    }
}
