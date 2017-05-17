// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="GuidSurrogate.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the GuidSurrogate class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.MscorlibSurrogates
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;



    public class GuidSurrogate
        : ISurrogate
    {

        public Type TargetType { get { return typeof(Guid); } }


        public void GetData(object value, SerializationInfo data)
        {
            if (!(value is Guid))
            {
                throw new ArgumentNullException(nameof(value));
            }

            var val = (Guid)value;
            var bytes = val.ToByteArray();
            data.AddValue("Length", bytes.Length);

            for (var n = 0; n < bytes.Length; n++)
            {
                data.AddValue(n.ToString(), bytes[n]);
            }
        }

        public void SetData(ref object value, SerializationInfo data)
        {
            var length = data.GetInt32("Length");

            var bytes = new byte[length];

            for (var n = 0; n < length; n++)
            {
                bytes[n] = data.GetByte(n.ToString());
            }

            var type = typeof(Guid).GetTypeInfo();
            value = new Guid(bytes);
        }
    }
}
