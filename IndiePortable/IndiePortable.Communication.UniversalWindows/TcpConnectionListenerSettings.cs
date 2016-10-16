// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpConnectionListenerSettings.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the TcpConnectionListenerSettings class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.UniversalWindows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Tcp;

    /// <summary>
    /// Provides information necessary for starting a <see cref="TcpConnectionListener" />.
    /// </summary>
    public sealed class TcpConnectionListenerSettings
    {
        /// <summary>
        /// The backing field for the <see cref="ListenerNetworkAdapters" /> property.
        /// </summary>
        private readonly IEnumerable<IPPortAddressInfo> listenerNetworkAdaptersBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnectionListenerSettings" /> class.
        /// </summary>
        /// <param name="listenerPort">
        ///     The TCP port that shall be listened on for incoming connections.
        /// </param>
        public TcpConnectionListenerSettings(ushort listenerPort)
        {
            this.listenerNetworkAdaptersBacking = new[] { new IPPortAddressInfo(new byte[4], listenerPort) };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnectionListenerSettings" /> class.
        /// </summary>
        /// <param name="listenerNetworkAdapters">
        ///     The network adapters' addresses to listen on.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="listenerNetworkAdapters" /> is <c>null</c>.</para>
        /// </exception>
        public TcpConnectionListenerSettings(params IPPortAddressInfo[] listenerNetworkAdapters)
            : this((IEnumerable<IPPortAddressInfo>)listenerNetworkAdapters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnectionListenerSettings" /> class.
        /// </summary>
        /// <param name="listenerNetworkAdapters">
        ///     The network adapters' addresses to listen on.
        ///     Must not be <c>null</c>.
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
        /// Gets the network adapters' addresses to listen on.
        /// </summary>
        /// <value>
        ///     Contains the network adapters' addresses to listen on.
        /// </value>
        public IEnumerable<IPPortAddressInfo> ListenerNetworkAdapters => this.listenerNetworkAdaptersBacking;
    }
}
