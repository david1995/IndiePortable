﻿// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpConnectionListener.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the TcpConnectionListener class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.UniversalWindows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Devices;
    using Tcp;
    using Windows.Networking;
    using Windows.Networking.Sockets;

    /// <summary>
    /// Listens for incoming connection over TCP.
    /// </summary>
    /// <seealso cref="Devices.IConnectionListener{TConnection, TSettings, TAddress}" />
    /// <seealso cref="IDisposable" />
    public sealed class TcpConnectionListener
        : IConnectionListener<TcpConnection, TcpConnectionListenerSettings, IPPortAddressInfo>, IDisposable
    {

        private readonly List<StreamSocketListener> listeners = new List<StreamSocketListener>();

        /// <summary>
        /// The backing field for the <see cref="IsListening" /> property.
        /// </summary>
        private bool isListeningBacking;


        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnectionListener" /> class.
        /// </summary>
        public TcpConnectionListener()
        {
        }
        
        /// <summary>
        /// Finalizes an instance of the <see cref="TcpConnectionListener" /> class.
        /// </summary>
        ~TcpConnectionListener()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Raised when a connection has been received.
        /// </summary>
        /// <remarks>
        ///     <para>Implements <see cref="IConnectionListener{TConnection, TSettings, TAddress}.ConnectionReceived" /> implicitly.</para>
        /// </remarks>
        public event EventHandler<ConnectionReceivedEventArgs<TcpConnection, IPPortAddressInfo>> ConnectionReceived;

        /// <summary>
        /// Gets a value indicating whether the <see cref="TcpConnectionListener" /> is actively listening for connections.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="TcpConnectionListener" />
        ///     is actively listening for connections; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="IConnectionListener{TConnection, TSettings, TAddress}.IsListening" /> implicitly.</para>
        /// </remarks>
        public bool IsListening => this.isListeningBacking;

        /// <summary>
        /// Starts listening for connections.
        /// </summary>
        /// <param name="settings">
        ///     The settings specifying parameters for the <see cref="TcpConnectionListener" />.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     <para>
        ///         Thrown if the <see cref="TcpConnectionListener" /> is already listening for connections.
        ///         Check the <see cref="IsListening" /> property.
        ///     </para>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="settings" /> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        ///     <para>Implements <see cref="IConnectionListener{TConnection, TSettings, TAddress}.StartListening(TSettings)" /> implicitly.</para>
        /// </remarks>
        public async void StartListening(TcpConnectionListenerSettings settings)
        {
            if (this.IsListening)
            {
                throw new InvalidOperationException();
            }

            if (object.ReferenceEquals(settings, null))
            {
                throw new ArgumentNullException(nameof(settings));
            }

            foreach (var ip in settings.ListenerNetworkAdapters)
            {
                var listener = new StreamSocketListener();
                await listener.BindEndpointAsync(new HostName(string.Join(".", ip.Address)), ip.Port.ToString());
                listener.ConnectionReceived += this.HandleListenerConnectionReceived;

                this.listeners.Add(listener);
            }

            this.isListeningBacking = true;
        }

        /// <summary>
        /// Stops listening for connections.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     <para>
        ///         Thrown if the <see cref="TcpConnectionListener" /> is not listening for incoming connections.
        ///         Check the <see cref="IsListening" /> property.
        ///     </para>
        /// </exception>
        /// <remarks>
        ///     <para>Implements <see cref="IConnectionListener{TConnection, TSettings, TAddress}.StopListening()" /> implicitly.</para>
        /// </remarks>
        public void StopListening()
        {
            if (!this.IsListening)
            {
                throw new InvalidOperationException();
            }

            this.listeners.ForEach(l => l.Dispose());
            this.listeners.Clear();

            this.isListeningBacking = false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        ///     <para>Implements <see cref="IDisposable.Dispose()" /> implicitly.</para>
        /// </remarks>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        private void HandleListenerConnectionReceived(object s, StreamSocketListenerConnectionReceivedEventArgs e)
        {
            var addressBytes = from sub in e.Socket.Information.RemoteAddress.RawName.Split('.')
                               select byte.Parse(sub);

            var ipAddressInfo = new IPPortAddressInfo(addressBytes.ToArray(), ushort.Parse(e.Socket.Information.LocalPort));
            var conn = new TcpConnection(e.Socket, ipAddressInfo);
        }

        /// <summary>
        /// Raises the <see cref="ConnectionReceived" /> event.
        /// </summary>
        /// <param name="connection">
        ///     The <see cref="TcpConnection" /> that has been received.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="connection" /> is <c>null</c>.</para>
        /// </exception>
        private void RaiseConnectionReceived(TcpConnection connection)
            => this.ConnectionReceived?.Invoke(this, new ConnectionReceivedEventArgs<TcpConnection, IPPortAddressInfo>(connection));

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                }

                this.listeners.ForEach(l => l.Dispose());
                this.listeners.Clear();
                this.isDisposed = true;
            }
        }
    }
}
