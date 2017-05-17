// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeSurrogate.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the DateTimeSurrogate class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter.MscorlibSurrogates
{
    using System;

    /// <summary>
    /// Provides serialization and de-serialization logic for the <see cref="DateTime" /> type.
    /// </summary>
    /// <seealso cref="ISurrogate" />
    internal sealed class DateTimeSurrogate
        : ISurrogate
    {
        /// <summary>
        /// Gets the target type for the <see cref="DateTimeSurrogate" />.
        /// </summary>
        /// <value>
        ///     Contains the target type for the <see cref="DateTimeSurrogate" />.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="ISurrogate.TargetType" /> implicitly.</para>
        /// </remarks>
        public Type TargetType
        {
            get { return typeof(DateTime); }
        }


        public void GetData(object value, ObjectDataCollection data)
        {
            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!(value is DateTime))
            {
                throw new ArgumentException(
                    "The value must be of type System.DateTime in order to be used with the DateTimeSurrogate.",
                    nameof(value));
            }

            var time = (DateTime)value;
            data.AddValue(nameof(DateTime.Ticks), time.Ticks);
        }


        public void SetData(ref object value, ObjectDataCollection data)
        {
            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!(value is DateTime))
            {
                throw new ArgumentException(
                    "The value must be of type System.DateTime in order to be used with the DateTimeSurrogate.",
                    nameof(value));
            }

            long ticks;
            if (!data.TryGetValue(nameof(DateTime.Ticks), out ticks))
            {
                throw new ArgumentException("The data collection must contain the ticks of the desired DateTime value.", nameof(data));
            }

            value = new DateTime(ticks);
        }
    }
}
