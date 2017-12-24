// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSpanSurrogate.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the TimeSpanSurrogate class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------


namespace IndiePortable.Formatter.MscorlibSurrogates
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides serialization and de-serialization logic for the <see cref="TimeSpan" /> type.
    /// </summary>
    /// <seealso cref="ISurrogate" />
    internal sealed class TimeSpanSurrogate
        : ISurrogate
    {
        /// <summary>
        /// Gets the target type for the <see cref="TimeSpanSurrogate" />.
        /// </summary>
        /// <value>
        ///     Contains the target type for the <see cref="TimeSpanSurrogate" />.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="ISurrogate.TargetType" /> implicitly.</para>
        /// </remarks>
        public Type TargetType
        {
            get { return typeof(TimeSpan); }
        }

        
        /// <inheritdoc />
        public void GetData(object value, SerializationInfo data)
        {
            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!(value is TimeSpan))
            {
                throw new ArgumentException(
                    "The value must be of type System.TimeSpan in order to be used with the TimeSpanSurrogate.",
                    nameof(value));
            }

            var time = (TimeSpan)value;
            data.AddValue(nameof(TimeSpan.Ticks), time.Ticks);
        }

        /// <inheritdoc />
        public void SetData(ref object value, SerializationInfo data)
        {
            if (object.ReferenceEquals(data, null))
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!(value is TimeSpan))
            {
                throw new ArgumentException(
                    "The value must be of type System.TimeSpan in order to be used with the TimeSpanSurrogate.",
                    nameof(value));
            }

            var ticks = data.GetInt64(nameof(TimeSpan.Ticks));
            value = new TimeSpan(ticks);
        }
    }
}
