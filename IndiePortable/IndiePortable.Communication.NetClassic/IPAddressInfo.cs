// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IPAddressInfo.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IPAddressInfo class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.NetClassic
{
    using System;
    using System.Net;
    using Devices;

    /// <summary>
    /// Encapsulates an <see cref="IPEndPoint" /> in an <see cref="IAddressInfo" />-compliant class.
    /// </summary>
    /// <seealso cref="IAddressInfo" />
    public class IPAddressInfo
        : IAddressInfo
    {
        /// <summary>
        /// The backing field for the <see cref="EndPoint" /> property.
        /// </summary>
        private readonly IPEndPoint endPointBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="IPAddressInfo" /> class.
        /// </summary>
        /// <param name="endPoint">
        ///     The <see cref="IPEndPoint" /> encapsulated by the <see cref="IPAddressInfo" />.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public IPAddressInfo(IPEndPoint endPoint)
        {
            if (object.ReferenceEquals(endPoint, null))
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            this.endPointBacking = endPoint;
        }

        /// <summary>
        /// Gets the <see cref="IPEndPoint" /> encapsulated by the <see cref="IPAddressInfo" />.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="IPEndPoint" /> encapsulated by the <see cref="IPAddressInfo" />.
        /// </value>
        public IPEndPoint EndPoint => this.endPointBacking;
    }
}
