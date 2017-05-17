// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSpanSurrogate.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the TimeSpanSurrogate class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------


namespace IndiePortable.Formatter.MscorlibSurrogates
{
    using System;

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

        /// <summary>
        /// Populates an <see cref="ObjectDataCollection" /> instance with data from a <see cref="TimeSpan" /> value.
        /// </summary>
        /// <param name="value">
        ///     The source <see cref="TimeSpan" /> providing the data.
        /// </param>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> that shall be populated with data.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if <paramref name="value" /> is not of type <see cref="TimeSpan" />.</para>
        /// </exception>
        public void GetData(object value, ObjectDataCollection data)
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

        /// <summary>
        /// Populates a <see cref="TimeSpan" /> value with data from an <see cref="ObjectDataCollection" /> instance.
        /// </summary>
        /// <param name="value">
        ///     The object that shall be populated with data. This parameter is to be passed by reference.
        /// </param>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> providing the data.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if <paramref name="value" /> is not of type <see cref="TimeSpan" />.</para>
        ///     <para>or</para>
        ///     <para><paramref name="data" /> does not contain the <see cref="TimeSpan.Ticks" /> property.</para>
        /// </exception>
        public void SetData(ref object value, ObjectDataCollection data)
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

            long ticks;
            if (!data.TryGetValue(nameof(TimeSpan.Ticks), out ticks))
            {
                throw new ArgumentException("The data collection must contain the ticks of the desired TimeSpan value.", nameof(data));
            }

            value = new TimeSpan(ticks);
        }
    }
}
