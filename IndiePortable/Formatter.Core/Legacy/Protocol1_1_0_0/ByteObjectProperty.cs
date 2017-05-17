// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteObjectProperty.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ByteObjectProperty class.
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

    
    public class ByteObjectProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteObjectProperty" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if:</para>
        ///     <para>  - <paramref name="name" /> is <c>null</c> or <see cref="string.Empty" />.</para>
        ///     <para>  - <paramref name="value" /> is <c>null</c>.</para>
        /// </exception>
        protected ByteObjectProperty(string name, int bodyLength, ObjectPropertyValue value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            
            this.Name = name;
            this.BodyLength = bodyLength;
            this.TotalLength = Encoding.UTF8.GetByteCount(name) + (sizeof(int) * 2) + bodyLength;
            this.Value = value;
        }


        public string Name { get; private set; }


        public int BodyLength { get; private set; }


        public int TotalLength { get; private set; }


        public ObjectPropertyValue Value { get; private set; }


        public static ByteObjectProperty FromStream(Stream source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.CanRead)
            {
                throw new ArgumentException("The specified stream cannot be read.", nameof(source));
            }

            // property name
            var propertyName = ObjectFormatter.GetStringFromStream(source, Encoding.UTF8);

            // property length
            var propertyLengthBytes = ObjectFormatter.ReadFromStream(source, sizeof(int));
            var propertyLength = BitConverter.ToInt32(propertyLengthBytes, 0);

            // property value
            var propertyValue = ObjectPropertyValue.FromStream(source);
            return new ByteObjectProperty(propertyName, propertyLength, propertyValue);
        }
    }
}
