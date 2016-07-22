// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="PrimitiveByteObject.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the PrimitiveByteObject class.
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



    public class PrimitiveByteObject
        : ByteObjectBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveByteObject" /> class.
        /// </summary>
        /// <param name="header">
        ///     The header of the <see cref="PrimitiveByteObject" />.
        /// </param>
        /// <param name="value">
        ///     The value of the <see cref="PrimitiveByteObject" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if:</para>
        ///     <para>  - <paramref name="header" /> is <c>null</c>. (by <see cref="ByteObjectBase(ByteObjectHeader)" />)</para>
        ///     <para>  - <paramref name="value" /> is <c>null</c>.</para>
        /// </exception>
        protected PrimitiveByteObject(ByteObjectHeader header, ValueType value)
            : base(header)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.Value = value;
        }

        
        public ValueType Value { get; private set; }


        public static PrimitiveByteObject FromStream(Stream source, ByteObjectHeader header)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (header == null)
            {
                throw new ArgumentNullException(nameof(header));
            }

            if (!source.CanRead)
            {
                throw new ArgumentException("The specified stream cannot be read.", nameof(source));
            }

            // read value
            var valueBytes = ObjectFormatter.ReadFromStream(source, header.Length);
            var value = ObjectFormatter.GetPrimitiveValue(valueBytes, header.PrimitiveType);

            return new PrimitiveByteObject(header, value);
        }
    }
}
