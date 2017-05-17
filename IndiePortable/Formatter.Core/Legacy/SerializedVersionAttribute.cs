// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializedVersionAttribute.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the SerializedVersionAttribute class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;

    /// <summary>
    /// Provides an attribute for defining versions for serializable types.
    /// </summary>
    /// <remarks>
    ///     Derives from <see cref="Attribute" />.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
    public sealed class SerializedVersionAttribute
        : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedVersionAttribute" /> class.
        /// </summary>
        /// <param name="major">
        ///     The major version number.
        /// </param>
        /// <param name="minor">
        ///     The minor version number.
        /// </param>
        /// <param name="build">
        ///     The build version number.
        /// </param>
        /// <param name="revision">
        ///     The revision version number.
        /// </param>
        public SerializedVersionAttribute(int major, int minor, int build = 0, int revision = 0)
        {
            this.Version = new Version(major, minor, build, revision);
            this.Major = major;
            this.Minor = minor;
            this.Build = build;
            this.Revision = revision;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedVersionAttribute" /> class.
        /// </summary>
        /// <param name="version">
        ///     The version of the type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="version" /> is null.
        /// </exception>
        public SerializedVersionAttribute(Version version)
        {
            // throw exception if version is null
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            this.Version = version;
            this.Major = version.Major;
            this.Minor = version.Minor;
            this.Build = version.Build;
            this.Revision = version.Revision;
        }

        /// <summary>
        /// Gets the <see cref="Version" /> of the type.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="Version" /> of the type.
        /// </value>
        public Version Version { get; private set; }

        /// <summary>
        /// Gets the major version number of the type.
        /// </summary>
        /// <value>
        ///     Contains the major version number of the type.
        /// </value>
        public int Major { get; private set; }

        /// <summary>
        /// Gets the minor version number of the type.
        /// </summary>
        /// <value>
        ///     Contains the minor version number of the type.
        /// </value>
        public int Minor { get; private set; }

        /// <summary>
        /// Gets the build version number of the type.
        /// </summary>
        /// <value>
        ///     Contains the build version number of the type.
        /// </value>
        public int Build { get; private set; }

        /// <summary>
        /// Gets the revision version number of the type.
        /// </summary>
        /// <value>
        ///     Contains the revision version number of the type.
        /// </value>
        public int Revision { get; private set; }
    }
}
