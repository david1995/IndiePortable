// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionSurrogate.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the CollectionSurrogate class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a surrogate for all <see cref="ICollection{T}" /> instances.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the collection values.
    /// </typeparam>
    public class CollectionSurrogate<T>
        : ISurrogate
    {
        /// <summary>
        /// Gets the target type for the <see cref="CollectionSurrogate{T}" />.
        /// </summary>
        /// <value>
        ///     Contains the target type for the <see cref="CollectionSurrogate{T}" />.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="ISurrogate.TargetType" /> implicitly.</para>
        /// </remarks>
        public Type TargetType { get { return typeof(ICollection<T>); } }

        /// <summary>
        /// The default <see cref="CollectionSurrogate{T}" />.
        /// </summary>
        public static readonly ISurrogate Default = new CollectionSurrogate<T>();

        /// <summary>
        /// Collects serialization data from an <see cref="ICollection{T}" />.
        /// </summary>
        /// <param name="value">
        ///     The <see cref="ICollection{T}" /> that shall be serialized.
        /// </param>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> that shall be populated.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if:</para>
        ///     <para>  - <paramref name="value" /> is <c>null</c>.</para>
        ///     <para>  - <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        ///     Implements <see cref="ISurrogate.GetData(object, ObjectDataCollection)" /> implicitly.
        /// </remarks>
        public void GetData(object value, ObjectDataCollection data)
        {
            var val = value as ICollection<T>;

            // throw exception if value is null
            if (val == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // throw exception if data is null
            if (data == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            data.AddValue("Count", val.Count);

            var index = 0;
            foreach (var elem in val)
            {
                data.AddValue(index.ToString(), elem);
                index++;
            }
        }

        /// <summary>
        /// Populates an <see cref="ICollection{T}" /> with data.
        /// </summary>
        /// <param name="value">
        ///     The <see cref="ICollection{T}" /> that shall be populated with values.
        /// </param>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> populated with information for <paramref name="value" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if:</para>
        ///     <para>  - <paramref name="value" /> is <c>null</c>.</para>
        ///     <para>  - <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        ///     Implements <see cref="ISurrogate.SetData(object, ObjectDataCollection)" /> implicitly.
        /// </remarks>
        public void SetData(ref object value, ObjectDataCollection data)
        {
            var val = value as ICollection<T>;

            // throw exception if value is null
            if (val == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // throw exception if data is null
            if (data == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var count = data.GetValue<int>("Count");

            for (var current = 0; current < count; current++)
            {
                val.Add(data.GetValue<T>(current.ToString()));
            }
        }
    }
}
