// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializedFieldVersionAttribute.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the SerializedFieldVersionAttribute class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;

    /// <summary>
    /// Provides an attribute for defining a compatible <see cref="Version" /> span for serializable fields.
    /// </summary>
    /// <remarks>
    ///     Derives from <see cref="Attribute" />.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class SerializedFieldVersionAttribute
        : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedFieldVersionAttribute" /> class.
        /// </summary>
        /// <param name="minVersionMajor">
        ///     The major number of the minimum version.
        /// </param>
        /// <param name="minVersionMinor">
        ///     The minor number of the minimum version.
        /// </param>
        /// <param name="minVersionBuild">
        ///     The build number of the minimum version.
        /// </param>
        /// <param name="minVersionRevision">
        ///     The revision number of the minimum version.
        /// </param>
        /// <param name="maxVersionMajor">
        ///     The major number of the maximum version.
        /// </param>
        /// <param name="maxVersionMinor">
        ///     The minor number of the maximum version.
        /// </param>
        /// <param name="maxVersionBuild">
        ///     The build number of the maximum version.
        /// </param>
        /// <param name="maxVersionRevision">
        ///     The revision number of the maximum version.
        /// </param>
        public SerializedFieldVersionAttribute(
            int minVersionMajor = 0,
            int minVersionMinor = 0,
            int minVersionBuild = 0,
            int minVersionRevision = 0,
            int maxVersionMajor = 0,
            int maxVersionMinor = 0,
            int maxVersionBuild = 0,
            int maxVersionRevision = 0)
            : this(
                  new Version(minVersionMajor, minVersionMinor, minVersionBuild, minVersionRevision),
                  new Version(maxVersionMajor, maxVersionMinor, maxVersionBuild, maxVersionRevision))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedFieldVersionAttribute" /> class.
        /// </summary>
        /// <param name="minVersion">
        ///     The minimum <see cref="Version" /> of the type that contains the field.
        /// </param>
        /// <param name="maxVersion">
        ///     The maximum <see cref="Version" /> of the type that contains the field.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown when both <paramref name="minVersion" /> and <paramref name="maxVersion" /> are <c>null</c>.</para>
        /// </exception>
        public SerializedFieldVersionAttribute(Version minVersion = null, Version maxVersion = null)
        {
            // throw exception if both min version and max version is null
            if (maxVersion == null && minVersion == null)
            {
                throw new ArgumentException("At least one version limit paramter has to be set to a value not equals null.", nameof(maxVersion));
            }

            this.MinVersion = minVersion;
            this.MaxVersion = maxVersion;

            this.MinVersionMajor = minVersion?.Major ?? 0;
            this.MinVersionMinor = minVersion?.Minor ?? 0;
            this.MinVersionBuild = minVersion?.Build ?? 0;
            this.MinVersionRevision = minVersion?.Revision ?? 0;

            this.MaxVersionMajor = maxVersion?.Major ?? 0;
            this.MinVersionMinor = maxVersion?.Minor ?? 0;
            this.MaxVersionBuild = maxVersion?.Build ?? 0;
            this.MaxVersionRevision = maxVersion?.Revision ?? 0;
        }

        /// <summary>
        /// Gets the minimum <see cref="Version" /> of the type that contains the field in order to (de-)serialize the value.
        /// </summary>
        /// <value>
        ///     Contains the minimum <see cref="Version" /> of the type that contains the field.
        /// </value>
        public Version MinVersion { get; private set; }

        /// <summary>
        /// Gets the maximum <see cref="Version" /> of the type that contains the field in order to (de-)serialize the value.
        /// </summary>
        /// <value>
        ///     Contains the maximum <see cref="Version" /> of the type that contains the field.
        /// </value>
        public Version MaxVersion { get; private set; }

        /// <summary>
        /// Gets the major number of the minimum supported <see cref="Version" />.
        /// </summary>
        /// <value>
        ///     Contains the major number of the minimum supported <see cref="Version" />.
        /// </value>
        public int MinVersionMajor { get; private set; }

        /// <summary>
        /// Gets the minor number of the minimum supported <see cref="Version" />.
        /// </summary>
        /// <value>
        ///     Contains the minor number of the minimum supported <see cref="Version" />.
        /// </value>
        public int MinVersionMinor { get; private set; }

        /// <summary>
        /// Gets the build number of the minimum supported <see cref="Version" />.
        /// </summary>
        /// <value>
        ///     Contains the build number of the minimum supported <see cref="Version" />.
        /// </value>
        public int MinVersionBuild { get; private set; }

        /// <summary>
        /// Gets the revision number of the minimum supported <see cref="Version" />.
        /// </summary>
        /// <value>
        ///     Contains the revision number of the minimum supported <see cref="Version" />.
        /// </value>
        public int MinVersionRevision { get; private set; }

        /// <summary>
        /// Gets the major number of the maximum supported <see cref="Version" />.
        /// </summary>
        /// <value>
        ///     Contains the major number of the maximum supported <see cref="Version" />.
        /// </value>
        public int MaxVersionMajor { get; private set; }

        /// <summary>
        /// Gets the minor number of the maximum supported <see cref="Version" />.
        /// </summary>
        /// <value>
        ///     Contains the minor number of the maximum supported <see cref="Version" />.
        /// </value>
        public int MaxVersionMinor { get; private set; }

        /// <summary>
        /// Gets the build number of the maximum supported <see cref="Version" />.
        /// </summary>
        /// <value>
        ///     Contains the build number of the maximum supported <see cref="Version" />.
        /// </value>
        public int MaxVersionBuild { get; private set; }

        /// <summary>
        /// Gets the revision number of the maximum supported <see cref="Version" />.
        /// </summary>
        /// <value>
        ///     Contains the revision number of the maximum supported <see cref="Version" />.
        /// </value>
        public int MaxVersionRevision { get; private set; }
    }
}
