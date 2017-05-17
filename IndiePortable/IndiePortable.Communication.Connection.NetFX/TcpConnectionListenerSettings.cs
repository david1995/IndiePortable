// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpConnectionListenerSettings.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contians the TcpConnectionListenerSettings class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Connection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    /// <summary>
    /// Encapsulates settings for a <see cref="TcpConnectionListener" />.
    /// </summary>
    public sealed class TcpConnectionListenerSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnectionListenerSettings"/> class.
        /// </summary>
        /// <param name="listenerPort">
        ///     The port that shall be listened to.
        /// </param>
        public TcpConnectionListenerSettings(ushort listenerPort)
        {
            this.ListenerNetworkAdapters = new[] { new IPEndPoint(IPAddress.Any, listenerPort) };
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
        public TcpConnectionListenerSettings(params IPEndPoint[] listenerNetworkAdapters)
            : this((IEnumerable<IPEndPoint>)listenerNetworkAdapters)
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
        public TcpConnectionListenerSettings(IEnumerable<IPEndPoint> listenerNetworkAdapters)
        {
            this.ListenerNetworkAdapters = listenerNetworkAdapters?.ToArray()
                                        ?? throw new ArgumentNullException(nameof(listenerNetworkAdapters));
        }

        /// <summary>
        /// Gets the network adaptes' address informations which shall be used to listen for incoming connections.
        /// </summary>
        /// <value>
        /// Contains the network adaptes' address informations which shall be used to listen for incoming connections.
        /// </value>
        public IEnumerable<IPEndPoint> ListenerNetworkAdapters { get; }
    }
}
