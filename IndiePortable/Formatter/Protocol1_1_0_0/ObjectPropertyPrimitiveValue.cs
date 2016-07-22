// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectPropertyPrimitiveValue.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ObjectPropertyPrimitiveValue class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Protocol1_1_0_0
{
    using System;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Represents the primitive value of a property.
    /// </summary>
    public class ObjectPropertyPrimitiveValue
        : ObjectPropertyValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectPropertyPrimitiveValue" /> class.
        /// </summary>
        /// <param name="value">
        ///     The <see cref="PrimitiveByteObject" /> representing the value.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="value" /> is <c>null</c>.</para>
        /// </exception>
        protected ObjectPropertyPrimitiveValue(PrimitiveByteObject value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.Value = value;
        }


        public PrimitiveByteObject Value { get; private set; }


        public static ObjectPropertyPrimitiveValue FromStream(Stream source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.CanRead)
            {
                throw new ArgumentException("The specified stream cannot be read.", nameof(source));
            }

            // read primitive value
            var value = ByteObjectBase.FromStream(source) as PrimitiveByteObject;
            if (value == null)
            {
                throw new InvalidOperationException("The read value is not a primitive value.");
            }

            return new ObjectPropertyPrimitiveValue(value);
        }
    }
}
