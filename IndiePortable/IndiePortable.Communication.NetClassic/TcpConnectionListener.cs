// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpConnectionListener.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the TcpConnectionListener class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.NetClassic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using AdvancedTasks;
    using Devices;
    using Tcp;

    /// <summary>
    /// Listens for incoming TCP connections.
    /// </summary>
    /// <seealso cref="Devices.IConnectionListener{TConnection, TSettings, TAddress}" />
    /// <seealso cref="IDisposable" />
    public sealed class TcpConnectionListener
        : IConnectionListener<TcpConnection, TcpConnectionListenerSettings, IPPortAddressInfo>, IDisposable
    {

        private readonly List<TcpConnectionListenerHelper> listeners = new List<TcpConnectionListenerHelper>();

        /// <summary>
        /// The backing field for the <see cref="IsListening" /> property.
        /// </summary>
        private bool isListeningBacking;

        /// <summary>
        /// Determines whether the <see cref="TcpConnectionListener" /> has been disposed.
        /// </summary>
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
        ///     <para>Thrown if the <see cref="TcpConnectionListener" /> is currently listening for connections.</para>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="settings" /> is <c>null</c>.</para>
        /// </exception>
        /// <remarks>
        ///     <para>Implements <see cref="IConnectionListener{TConnection, TSettings, TAddress}.StartListening(TSettings)" /> implicitly.</para>
        /// </remarks>
        public void StartListening(TcpConnectionListenerSettings settings)
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
                var l = new TcpConnectionListenerHelper(new TcpListener(new IPEndPoint(new IPAddress(ip.Address), ip.Port)));
                l.ConnectionReceived += (s, e) => this.RaiseConnectionReceived(e.ReceivedConnection);
                this.listeners.Add(l);
                l.Start();
            }

            this.isListeningBacking = true;
        }

        /// <summary>
        /// Stops listening for connections.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the <see cref="TcpConnectionListener" /> is not listening for connections.</para>
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

            this.listeners.ForEach(l => l.Stop());
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

        /// <summary>
        /// Raises the <see cref="ConnectionReceived" /> event.
        /// </summary>
        /// <param name="connection">
        ///     The <see cref="TcpConnection" /> that has been received.
        ///     Must not be <c>null</c>.
        /// </param>
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
                    this.listeners.Clear();
                }

                this.listeners.ForEach(l => l.Dispose());
                this.isListeningBacking = false;
                this.isDisposed = true;
            }
        }


        private sealed class TcpConnectionListenerHelper
            : IDisposable
        {

            private readonly TcpListener listener;


            private StateTask listenerTask;


            private bool isDisposed;

            /// <summary>
            /// The backing field for the <see cref="IsActive" /> property.
            /// </summary>
            private bool isActiveBacking;

            /// <summary>
            /// Initializes a new instance of the <see cref="TcpConnectionListenerHelper"/> class.
            /// </summary>
            /// <param name="listener">
            ///     The <see cref="TcpListener" /> that shall be listened with.
            ///     Must not be <c>null</c>.
            /// </param>
            /// <exception cref="ArgumentNullException">
            ///     <para>Thrown if <paramref name="listener" /> is <c>null</c>.</para>
            /// </exception>
            public TcpConnectionListenerHelper(TcpListener listener)
            {
                if (object.ReferenceEquals(listener, null))
                {
                    throw new ArgumentNullException(nameof(listener));
                }

                this.listener = listener;
            }

            /// <summary>
            /// Finalizes an instance of the <see cref="TcpConnectionListenerHelper" /> class.
            /// </summary>
            ~TcpConnectionListenerHelper()
            {
                this.Dispose(false);
            }


            public event EventHandler<ConnectionReceivedEventArgs<TcpConnection, IPPortAddressInfo>> ConnectionReceived;


            public bool IsActive => this.isActiveBacking;


            public void Start()
            {
                if (this.IsActive)
                {
                    throw new InvalidOperationException();
                }

                this.listener.Start();
                this.listenerTask = new StateTask(this.ConnectionListenerTask);
                this.isActiveBacking = true;
            }


            public void Stop()
            {
                if (!this.IsActive)
                {
                    throw new InvalidOperationException();
                }

                this.listenerTask.Stop();
                this.listener.Stop();
                this.isActiveBacking = false;
            }


            private void RaiseConnectionReceived(TcpConnection connection)
                => this.ConnectionReceived?.Invoke(this, new ConnectionReceivedEventArgs<TcpConnection, IPPortAddressInfo>(connection));


            private async void ConnectionListenerTask(ITaskConnection connection)
            {
                if (object.ReferenceEquals(connection, null))
                {
                    throw new ArgumentNullException(nameof(connection));
                }

                try
                {
                    while (!connection.MustFinish)
                    {
                        while (this.listener.Pending())
                        {
                            var cl = await this.listener.AcceptTcpClientAsync();
                            var remoteEP = (IPEndPoint)cl.Client.RemoteEndPoint;
                            var conn = new TcpConnection(cl, new IPPortAddressInfo(remoteEP.Address.GetAddressBytes(), (ushort)remoteEP.Port));
                            this.RaiseConnectionReceived(conn);
                        }

                        await Task.Delay(50);
                    }
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception exc)
                {
                    connection.ThrowException(exc);
                    return;
                }

                connection.Return();
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

                    this.listenerTask.StopAndAwait();

                    if (this.IsActive)
                    {
                        this.listener.Stop();
                    }

                    this.isDisposed = true;
                }
            }
        }
    }
}
