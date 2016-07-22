// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteObjectBase.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ByteObjectBase class.
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



    public abstract class ByteObjectBase
    {

        protected ByteObjectBase(ByteObjectHeader header)
        {
            if (header == null)
            {
                throw new ArgumentNullException(nameof(header));
            }

            this.Header = header;
        }


        public ByteObjectHeader Header { get; private set; }


        public static ByteObjectBase FromStream(Stream source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.CanRead)
            {
                throw new ArgumentException("The specified stream cannot be read.", nameof(source));
            }

            var header = ByteObjectHeader.FromStream(source);

            switch (header.MajorType)
            {
                case ObjectType.RefType: return RefByteObject.FromStream(source, header);

                case ObjectType.String:
                        
                case ObjectType.Primitive:

                case ObjectType.ValueType:

                default: throw new InvalidOperationException();
            }
        }
    }
}
