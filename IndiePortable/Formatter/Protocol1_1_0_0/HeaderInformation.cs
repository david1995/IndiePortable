// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderInformation.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the HeaderInformation class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------
namespace IndiePortable.Formatter.Protocol1_1_0_0
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    
    public class HeaderInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderInformation" /> class.
        /// </summary>
        /// <param name="intro">
        ///     The intro of the package.
        /// </param>
        /// <param name="length">
        ///     The length of the package.
        /// </param>
        /// <param name="packageId">
        ///     The package identifier of the read package.
        /// </param>
        /// <param name="partId">
        ///     The part identifier of the package part.
        /// </param>
        /// <param name="partCount">
        ///     The part count of the package.
        /// </param>
        /// <param name="headerVersion">
        ///     The version of the header.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="headerVersion" /> is <c>null</c>.</para>
        /// </exception>
        protected HeaderInformation(int intro, int length, int packageId, int partId, int partCount, Version headerVersion)
        {
            if (headerVersion == null)
            {
                throw new ArgumentNullException(nameof(headerVersion));
            }

            this.Intro = intro;
            this.Length = length;
            this.PackageId = packageId;
            this.PartId = partId;
            this.PartCount = partCount;
            this.HeaderVersion = headerVersion;
        }


        public int Intro { get; private set; }


        public int Length { get; private set; }


        public int PackageId { get; private set; }


        public int PartId { get; private set; }


        public int PartCount { get; private set; }


        public Version HeaderVersion { get; private set; }


        public static HeaderInformation FromStream(Stream source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.CanRead)
            {
                throw new ArgumentException("The specified stream cannot be read.", nameof(source));
            }

            // intro
            var introBytes = ObjectFormatter.ReadFromStream(source, sizeof(int));
            var intro = BitConverter.ToInt32(introBytes, 0);

            // part length
            var lengthBytes = ObjectFormatter.ReadFromStream(source, sizeof(int));
            var length = BitConverter.ToInt32(lengthBytes, 0);

            // package id
            var packageIdBytes = ObjectFormatter.ReadFromStream(source, sizeof(int));
            var packageId = BitConverter.ToInt32(packageIdBytes, 0);

            // part id
            var partIdBytes = ObjectFormatter.ReadFromStream(source, sizeof(int));
            var partId = BitConverter.ToInt32(partIdBytes, 0);

            // part count
            var partCountBytes = ObjectFormatter.ReadFromStream(source, sizeof(int));
            var partCount = BitConverter.ToInt32(partCountBytes, 0);

            // version
            var majorVersionBytes = ObjectFormatter.ReadFromStream(source, sizeof(int));
            var minorVersionBytes = ObjectFormatter.ReadFromStream(source, sizeof(int));
            var buildVersionBytes = ObjectFormatter.ReadFromStream(source, sizeof(int));
            var releaseVersionBytes = ObjectFormatter.ReadFromStream(source, sizeof(int));

            var version = new Version(
                BitConverter.ToInt32(majorVersionBytes, 0),
                BitConverter.ToInt32(minorVersionBytes, 0),
                BitConverter.ToInt32(buildVersionBytes, 0),
                BitConverter.ToInt32(releaseVersionBytes, 0));

            return new HeaderInformation(intro, length, packageId, partId, partCount, version);
        }
    }
}
