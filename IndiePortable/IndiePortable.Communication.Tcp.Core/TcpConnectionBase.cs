// <copyright file="TcpConnectionBase.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Tcp.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using IndiePortable.AdvancedTasks;
    using IndiePortable.Communication.Core.Devices;
    using IndiePortable.Communication.Core.Devices.ConnectionMessages;
    using IndiePortable.Communication.Core.Messages;

    public abstract class TcpConnectionBase
        : IConnection<IPEndPoint>
    {
        private ConnectionState connectionState;

        private CryptoManagerBase<PublicKeyInfo> cryptoManager;

        protected TcpConnectionBase(IPEndPoint remoteAddress, CryptoManagerBase<PublicKeyInfo> cryptoManager)
        {
            this.ConnectionState = ConnectionState.Initializing;

            this.RemoteAddress = remoteAddress ?? throw new ArgumentNullException(nameof(remoteAddress));
            this.cryptoManager = cryptoManager ?? throw new ArgumentNullException(nameof(cryptoManager));
            this.ConnectionStateChanged += (s, e) =>
            {
                if (this.ConnectionState == ConnectionState.Disconnected)
                {
                    this.OnDisconnected();
                }
            };

            this.ConnectionState = ConnectionState.Initialized;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TcpConnectionBase"/> class.
        /// </summary>
        ~TcpConnectionBase()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Raised when the <see cref="TcpConnectionBase" /> has been disconnected.
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        /// Raised when a <see cref="MessageBase" /> object has been received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Occurs when the <see cref="ConnectionState" /> property has been changed.
        /// </summary>
        public event EventHandler ConnectionStateChanged;

        public bool IsConnected => this.ConnectionState != ConnectionState.Lost
                                && this.connectionState != ConnectionState.Disconnecting
                                && this.ConnectionState != ConnectionState.Disconnected
                                && this.ConnectionState != ConnectionState.Disposing
                                && this.ConnectionState != ConnectionState.Disposed;

        public bool IsActivated => this.connectionState == ConnectionState.Activated;

        public IPEndPoint RemoteAddress { get; }

        public MessageDispatcher Cache { get; }

        public ConnectionState ConnectionState
        {
            get => this.connectionState;
            private set
            {
                this.connectionState = value;
                this.OnConnectionStateChanged();
            }
        }

        public MessageDispatcher MessageCache { get; }

        protected ConnectionMessageDispatcher<IPEndPoint> ConnectionMessageCache { get; }

        public void Activate()
        {
            if (this.ConnectionState != ConnectionState.Initialized)
            {
                throw new InvalidOperationException($"Invalid state; {ConnectionState.Initialized} expected.");
            }

            throw new NotImplementedException();
            this.ConnectionState = ConnectionState.Activating;

            this.ConnectionState = ConnectionState.Activated;
        }

        public void Disconnect()
        {
            this.ConnectionState = ConnectionState.Disconnecting;

            var rq = new ConnectionDisconnectRequest();
            this.SendConnectionMessage(new ConnectionDisconnectRequest());

            this.ConnectionState = ConnectionState.Disconnected;
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SendMessage(MessageBase message)
            => this.SendConnectionMessage(
                new ConnectionContentMessage(
                    message ?? throw new ArgumentNullException(nameof(message))));

        public async Task SendMessageAsync(MessageBase message)
            => await this.SendConnectionMessageAsync(
                new ConnectionContentMessage(
                    message ?? throw new ArgumentNullException(nameof(message))));

        protected abstract void SendConnectionMessage(
            ConnectionMessageBase message,
            CryptoManagerBase<PublicKeyInfo> cryptoManager);

        protected abstract Task SendConnectionMessageAsync(
            ConnectionMessageBase message,
            CryptoManagerBase<PublicKeyInfo> cryptoManager);

        protected virtual void Dispose(bool disposing)
        {
            if (this.ConnectionState != ConnectionState.Disposed &&
                this.ConnectionState != ConnectionState.Disposing)
            {
                this.ConnectionState = ConnectionState.Disposing;
                if (disposing)
                {
                }

                this.ConnectionState = ConnectionState.Disposed;
            }
        }

        /// <summary>
        /// Called when the <see cref="ConnectionState"/> property has been changed.
        /// </summary>
        protected virtual void OnConnectionStateChanged()
            => this.ConnectionStateChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Called when the <see cref="ConnectionState"/> is disconnected.
        /// </summary>
        protected virtual void OnDisconnected()
            => this.Disconnected?.Invoke(this, EventArgs.Empty);

        protected abstract void MessageReader(ITaskConnection taskConnection);

        private async void KeepAliveChecker(ITaskConnection connection)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            try
            {
                while (!connection.MustFinish)
                {
                    if (!await Task.Factory.StartNew(() => this.keepAliveWaitHandle.WaitOne(this.MaxKeepAliveTimeout)))
                    {
                        try
                        {
                            this.Disconnect();
                        }
                        catch (IOException)
                        {
                        }
                        finally
                        {
                            this.keepAliveCheckerTask.Stop();
                            this.Dispose();
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
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

        private async void KeepAliveSender(ITaskConnection connection)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            try
            {
                while (!connection.MustFinish)
                {
                    await this.SendConnectionMessageAsync(
                        new ConnectionMessageKeepAlive(),
                        this.cryptoManager);

                    await Task.Delay(this.KeepAliveFrequency);
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
    }
}
