// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpConnectionListenerSettings.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contians the TcpConnectionListenerSettings class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.NetClassic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Tcp;

    /// <summary>
    /// Encapsulates settings for a <see cref="TcpConnectionListener" />.
    /// </summary>
    public sealed class TcpConnectionListenerSettings
    {
        /// <summary>
        /// The backing field for the <see cref="ListenerNetworkAdapters" /> property.
        /// </summary>
        private readonly IEnumerable<IPPortAddressInfo> listenerNetworkAdaptersBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnectionListenerSettings"/> class.
        /// </summary>
        /// <param name="listenerPort">
        ///     The port that shall be listened to.
        /// </param>
        public TcpConnectionListenerSettings(ushort listenerPort)
        {
            this.listenerNetworkAdaptersBacking = new[] { new IPPortAddressInfo(IPAddress.Any.GetAddressBytes(), listenerPort) };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnectionListenerSettings"/> class.
        /// </summary>
        /// <param name="listenerNetworkAdapters">
        ///     The network adaptes' address informations which shall be used to listen for incoming connections.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="listenerNetworkAdapters" /> is <c>null</c>.</para>
        /// </exception>
        public TcpConnectionListenerSettings(params IPPortAddressInfo[] listenerNetworkAdapters)
            : this((IEnumerable<IPPortAddressInfo>)listenerNetworkAdapters)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnectionListenerSettings"/> class.
        /// </summary>
        /// <param name="listenerNetworkAdapters">
        ///     The network adaptes' address informations which shall be used to listen for incoming connections.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="listenerNetworkAdapters" /> is <c>null</c>.</para>
        /// </exception>
        public TcpConnectionListenerSettings(IEnumerable<IPPortAddressInfo> listenerNetworkAdapters)
        {
            if (object.ReferenceEquals(listenerNetworkAdapters, null))
            {
                throw new ArgumentNullException(nameof(listenerNetworkAdapters));
            }

            this.listenerNetworkAdaptersBacking = listenerNetworkAdapters.ToArray();
        }

        /// <summary>
        /// Gets the network adaptes' address informations which shall be used to listen for incoming connections.
        /// </summary>
        /// <value>
        ///     Contains the network adaptes' address informations which shall be used to listen for incoming connections.
        /// </value>
        public IEnumerable<IPPortAddressInfo> ListenerNetworkAdapters => this.listenerNetworkAdaptersBacking;
    }
}
