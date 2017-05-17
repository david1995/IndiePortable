// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="RefByteObject.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the RefByteObject class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.Protocol1_1_0_0
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    public class RefByteObject
        : ByteObjectBase
    {
        
        public RefByteObject(ByteObjectHeader header, IEnumerable<ByteObjectProperty> properties)
            : base(header)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            this.Properties = new ReadOnlyCollection<ByteObjectProperty>(properties.ToArray());
        }
        

        public IReadOnlyList<ByteObjectProperty> Properties { get; private set; }



        public static RefByteObject FromStream(Stream source, ByteObjectHeader header)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (header == null)
            {
                throw new ArgumentNullException(nameof(header));
            }

            var currentLength = 0;

            while (currentLength < header.Length)
            {
                var property = ByteObjectProperty.FromStream(source);
                currentLength += property.TotalLength;

                
            }

            throw new NotImplementedException();
        }
    }
}
