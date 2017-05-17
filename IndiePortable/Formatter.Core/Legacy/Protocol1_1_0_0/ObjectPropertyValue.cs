// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectPropertyValue.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ObjectPropertyValue class.
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


    public abstract class ObjectPropertyValue
    {

        
        public static ObjectPropertyValue FromStream(Stream source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.CanRead)
            {
                throw new ArgumentException("The specified stream cannot be read.", nameof(source));
            }
            
            // read object type
            var objectTypeBytes = ObjectFormatter.ReadFromStream(source, sizeof(int));
            var majorType = (ObjectType)objectTypeBytes[0];
            var primitiveType = (ObjectType)objectTypeBytes[1];

            switch (majorType)
            {
                case ObjectType.RefType: return ObjectPropertyRefValue.FromStream(source);

                case ObjectType.ValueType: 

                case ObjectType.String:

                case ObjectType.Enum: 

                case ObjectType.Primitive: return ObjectPropertyPrimitiveValue.FromStream(source);

                case ObjectType.Null: break;
            }

            throw new NotImplementedException();
        }
    }
}
