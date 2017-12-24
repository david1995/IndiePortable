// <copyright file="StreamTcpConnectionListener.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// TODO: make documentation

using System;
using System.Net;
using System.Net.Sockets;
using IndiePortable.Communication.Binary;

namespace IndiePortable.Communication.Tcp
{

    public class StreamTcpConnectionListener
        : IStreamConnectionListener<StreamTcpConnection, IPEndPoint>
    {
        private readonly TcpListener listener;

        private bool isDisposed;

        public StreamTcpConnectionListener(int localPort)
            : this(new IPEndPoint(
                port: localPort < IPEndPoint.MinPort || localPort >= IPEndPoint.MaxPort
                    ? throw new ArgumentOutOfRangeException(nameof(localPort))
                    : localPort,
                address: IPAddress.Any))
        {
        }

        public StreamTcpConnectionListener(IPEndPoint localAddress)
        {
            this.listener = new TcpListener(localAddress ?? throw new ArgumentNullException(nameof(localAddress)));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="StreamTcpConnectionListener"/> class.
        /// </summary>
        ~StreamTcpConnectionListener()
        {
            this.Dispose(false);
        }

        public event StreamConnectionAcceptedEventHandler ConnectionRequested;

        public bool IsListening { get; private set; }

        public IPEndPoint LocalAddress => this.listener.LocalEndpoint as IPEndPoint;

        public void StartListening()
        {
            if (this.IsListening)
            {
                throw new InvalidOperationException();
            }

            this.listener.Start();
            this.IsListening = true;
        }

        public void StopListening()
        {
            if (!this.IsListening)
            {
                throw new InvalidOperationException();
            }

            this.listener.Stop();
            this.IsListening = false;
        }

        /// <inheritdoc />
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
            }

            this.isDisposed = true;
        }
    }
}
