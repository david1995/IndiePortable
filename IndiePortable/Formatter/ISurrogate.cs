// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ISurrogate.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ISurrogate interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides a surrogate for types that provide no serialization mechanism.
    /// </summary>
    public interface ISurrogate
    {
        /// <summary>
        /// Gets the target type for the <see cref="ISurrogate" />.
        /// </summary>
        /// <value>
        ///     Contains the target type for the <see cref="ISurrogate" />.
        /// </value>
        Type TargetType { get; }

        /// <summary>
        /// Populates an <see cref="SerializationInfo" /> instance with data from an object.
        /// </summary>
        /// <param name="value">
        ///     The source object providing the data.
        /// </param>
        /// <param name="data">
        ///     The <see cref="SerializationInfo" /> that shall be populated with data.
        /// </param>
        void GetData(object value, SerializationInfo data);

        /// <summary>
        /// Populates an object with data from an <see cref="SerializationInfo" /> instance.
        /// </summary>
        /// <param name="value">
        ///     The object that shall be populated with data. This parameter is to be passed by reference.
        /// </param>
        /// <param name="data">
        ///     The <see cref="SerializationInfo" /> providing the data.
        /// </param>
        void SetData(ref object value, SerializationInfo data);
    }
}
