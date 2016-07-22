using System;
using System.Collections.Generic;
// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="CultureInfoSurrogate.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the CultureInfoSurrogate class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.MscorlibSurrogates
{
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides serialization and de-serialization logic for the <see cref="CultureInfo" /> type.
    /// </summary>
    /// <seealso cref="ISurrogate" />
    internal sealed class CultureInfoSurrogate
        : ISurrogate
    {

        public Type TargetType { get { return typeof(CultureInfo); } }


        public void GetData(object value, ObjectDataCollection data)
        {
            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }
            
            var val = value as CultureInfo;
            if (val == null)
            {
                throw new ArgumentException(nameof(value));
            }

            data.AddValue(nameof(CultureInfo.Name), val.Name);
        }

        public void SetData(ref object value, ObjectDataCollection data)
        {
            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            var val = value as CultureInfo;
            if (val == null)
            {
                throw new ArgumentException(nameof(value));
            }

            string name;
            if (!data.TryGetValue(nameof(CultureInfo.Name), out name))
            {
                throw new ArgumentException();
            }

            var info = new CultureInfo(name);

            var fields = from f in typeof(CultureInfo).GetTypeInfo().DeclaredFields
                         where !f.IsStatic && !f.IsInitOnly
                         select f;

            foreach (var field in fields)
            {
                field.SetValue(value, field.GetValue(info));
            }
        }
    }
}
