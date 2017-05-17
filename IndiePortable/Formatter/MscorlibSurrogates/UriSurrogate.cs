// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="UriSurrogate.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the UriSurrogate class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.MscorlibSurrogates
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    internal sealed class UriSurrogate
        : ISurrogate
    {

        private readonly TypeInfo targetTypeInfo = typeof(Uri).GetTypeInfo();


        private readonly ConstructorInfo constructor;


        public UriSurrogate()
        {
            var constructors = from c in this.targetTypeInfo.DeclaredConstructors
                               let p = c.GetParameters()
                               where p.Length == 1 && p[0].ParameterType == typeof(string)
                               select c;

            this.constructor = constructors.Single();
        }


        public Type TargetType => typeof(Uri);


        public void GetData(object value, SerializationInfo data)
        {
            if (object.ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            var uri = value as Uri;
            if (object.ReferenceEquals(uri, null))
            {
                throw new ArgumentException();
            }

            data.AddValue("AsString", uri.ToString());
        }


        public void SetData(ref object value, SerializationInfo data)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!(value is Uri))
            {
                throw new ArgumentException();
            }

            var asString = data.GetString("AsString");

            this.constructor.Invoke(value, new object[] { asString });
        }
    }
}
