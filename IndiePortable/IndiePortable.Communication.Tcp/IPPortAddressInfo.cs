// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IPPortAddressInfo.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IPPortAddressInfo class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Tcp
{
    using System;
    using Devices;
    using Formatter;

    /// <summary>
    /// Represents IP address and port of a TCP-capable network device.
    /// </summary>
    /// <seealso cref="IAddressInfo" />
    [Serializable]
    public sealed class IPPortAddressInfo
        : IAddressInfo
    {
        /// <summary>
        /// The backing field for the <see cref="Address" /> property.
        /// </summary>
        [Serialized]
        private byte[] addressBacking;

        /// <summary>
        /// The backing field for the <see cref="Port" /> property.
        /// </summary>
        [Serialized]
        private ushort portBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="IPPortAddressInfo"/> class.
        /// </summary>
        /// <param name="address">
        ///     The IP address in IPv4 or IPv6 format.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <param name="port">
        ///     The port number.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="address" /> is <c>null</c>.</para>
        /// </exception>
        public IPPortAddressInfo(byte[] address, ushort port)
        {
            if (object.ReferenceEquals(address, null))
            {
                throw new ArgumentNullException(nameof(address));
            }

            this.addressBacking = address;
            this.portBacking = port;
        }

        /// <summary>
        /// Gets the IP address in IPv4 or IPv6 format.
        /// </summary>
        /// <value>
        ///     Contains the IP address in IPv4 or IPv6 format.
        /// </value>
        public byte[] Address => this.addressBacking;

        /// <summary>
        /// Gets the port number.
        /// </summary>
        /// <value>
        ///     Contains the port number.
        /// </value>
        public ushort Port => this.portBacking;
    }
}
