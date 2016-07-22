// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectPropertyRefValue.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file conatins the ObjectPropertyRefValue class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------
namespace IndiePortable.Formatter.Protocol1_1_0_0
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents a binary object reference value of a property.
    /// </summary>
    /// <seealso cref="ObjectPropertyValue" />
    public class ObjectPropertyRefValue
        : ObjectPropertyValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectPropertyRefValue" /> class.
        /// </summary>
        /// <param name="reference">
        ///     The object reference stored in the property.
        /// </param>
        protected ObjectPropertyRefValue(int reference)
        {
            this.Reference = reference;
        }

        /// <summary>
        /// Gets the object reference stored in the property.
        /// </summary>
        /// <value>
        ///     Contains the object reference stored in the property.
        /// </value>
        public int Reference { get; private set; }

        /// <summary>
        /// Builds a <see cref="ObjectPropertyRefValue" /> from a stream source.
        /// </summary>
        /// <param name="source">
        ///     The <see cref="Stream" /> serving as source.
        /// </param>
        /// <returns>
        ///     Returns the read <see cref="ObjectPropertyRefValue" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="source" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if <paramref name="source" /> cannot be read.</para>
        /// </exception>
        public static ObjectPropertyRefValue FromStream(Stream source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.CanRead)
            {
                throw new ArgumentException("The specified stream cannot be read.", nameof(source));
            }

            // read object reference value
            var referenceBytes = ObjectFormatter.ReadFromStream(source, sizeof(int));
            var reference = BitConverter.ToInt32(referenceBytes, 0);

            return new ObjectPropertyRefValue(reference);
        }
    }
}
