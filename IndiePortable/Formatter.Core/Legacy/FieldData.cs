// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldData.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the FieldData class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Provides information about the value of a field in a <see cref="Type" />.
    /// </summary>
    public class FieldData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldData" /> class.
        /// </summary>
        /// <param name="key">
        ///     The name of the field.
        /// </param>
        /// <param name="value">
        ///     The value of the field.
        /// </param>
        /// <param name="minVersion">
        ///     The minimum supported version of the field.
        /// </param>
        /// <param name="maxVersion">
        ///     The maximum supported version of the field.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="key" /> is null or empty.
        /// </exception>
        public FieldData(string key, object value, Version minVersion = null, Version maxVersion = null)
        {
            // throw exception if key is null or emtpy
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), $"The {nameof(key)} parameter must not be null or empty.");
            }
            
            this.Key = key;
            this.Value = value;
            this.MinVersion = minVersion;
            this.MaxVersion = maxVersion;
        }

        /// <summary>
        /// Gets the key of the <see cref="FieldData" />.
        /// </summary>
        /// <value>
        ///     Contains the key of the <see cref="FieldData" />.
        /// </value>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the value of the <see cref="FieldData" />.
        /// </summary>
        /// <value>
        ///     Contains the value of the <see cref="FieldData" />.
        /// </value>
        public object Value { get; private set; }

        /// <summary>
        /// Gets the minimum version of the <see cref="FieldData" />.
        /// </summary>
        /// <value>
        ///     Contains the minimum version of the <see cref="FieldData" />.
        /// </value>
        public Version MinVersion { get; private set; }

        /// <summary>
        /// Gets the maximum version of the <see cref="FieldData" />.
        /// </summary>
        /// <value>
        ///     Contains the maximum version of the <see cref="FieldData" />.
        /// </value>
        public Version MaxVersion { get; private set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        /// <remarks>
        ///     Overrides <see cref="object.ToString()" />.
        /// </remarks>
        public override string ToString() => $@"Key: ""{this.Key}"", Value: ""{this.Value}""";
    }
}
