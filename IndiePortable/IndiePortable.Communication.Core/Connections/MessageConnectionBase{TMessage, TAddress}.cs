// <copyright file="MessageConnectionBase{TMessage, TAddress}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core.Devices.Connections
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class MessageConnectionBase<TMessage, TAddress>
        : IDisposable
        where TMessage : class
    {
        private readonly object sendLock = new object();

        private ConnectionState connectionState;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageConnectionBase{TMessage, TAddress}"/> class.
        /// </summary>
        protected MessageConnectionBase()
        {
            this.ConnectionStateChanged += (s, e) =>
            {
                if (this.ConnectionState == ConnectionState.Disconnected)
                {
                    this.OnDisconnected();
                }
            };
        }

        /// <summary>
        /// Occurs when the <see cref="MessageConnectionBase{TMessage, TAddress}" /> has been disconnected.
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        /// Occurs when the <see cref="ConnectionState"/> property has been changed.
        /// </summary>
        public event EventHandler ConnectionStateChanged;

        public event EventHandler<MessageReceivedEventArgs<TMessage>> MessageReceived;

        public ConnectionState ConnectionState
        {
            get => this.connectionState;
            protected set
            {
                this.connectionState = value;
                this.OnConnectionStateChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="MessageConnectionBase{TMessage, TAddress}" /> is connected to the other end.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="MessageConnectionBase{TMessage, TAddress}" /> is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected => this.ConnectionState != ConnectionState.Lost
                                && this.connectionState != ConnectionState.Disconnecting
                                && this.ConnectionState != ConnectionState.Disconnected
                                && this.ConnectionState != ConnectionState.Disposing
                                && this.ConnectionState != ConnectionState.Disposed;

        /// <summary>
        /// Gets a value indicating whether the <see cref="MessageConnectionBase{TMessage, TAddress}" /> is activated.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="MessageConnectionBase{TMessage, TAddress}" /> is activated; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// <para>
        /// Messages can only be sent or received if <see cref="IsActivated" /> is <c>true</c>.
        /// Otherwise, an <see cref="InvalidOperationException" /> will be thrown.
        /// </para>
        /// </remarks>
        public bool IsActivated => this.ConnectionState == ConnectionState.Activated;

        /// <summary>
        /// Gets the remote address of the other connection end.
        /// </summary>
        /// <value>
        /// Contains the remote address of the other connection end.
        /// </value>
        public abstract TAddress RemoteAddress { get; }

        /// <summary>
        /// Activates the <see cref="MessageConnectionBase{TMessage, TAddress}" />.
        /// </summary>
        /// <remarks>
        /// <para>Call this method to allow incoming and outgoing messages to be sent or received.</para>
        /// </remarks>
        public void Activate()
        {
            if (this.ConnectionState != ConnectionState.Initialized)
            {
                throw new InvalidOperationException($"Invalid state; {ConnectionState.Initialized} expected.");
            }

            this.ConnectionState = ConnectionState.Activating;
            this.ActivateOverride();
            this.ConnectionState = ConnectionState.Activated;
        }

        /// <summary>
        /// Disconnects the two end points.
        /// </summary>
        public void Disconnect()
        {
            lock (this.sendLock)
            {
                if (this.ConnectionState != ConnectionState.Activated)
                {
                    throw new InvalidOperationException($"Invalid state; {ConnectionState.Activated} expected.");
                }

                this.ConnectionState = ConnectionState.Disconnecting;
                this.DisconnectOverride();
                this.ConnectionState = ConnectionState.Disconnected;
            }
        }

        public async Task DisconnectAsync()
        {
            await Task.Run(() => Monitor.Wait(this.sendLock));
            try
            {
                this.ConnectionState = ConnectionState.Disconnecting;
                await this.DisconnectAsyncOverride();
                this.ConnectionState = ConnectionState.Disconnected;
            }
            finally
            {
                Monitor.Exit(this.sendLock);
            }
        }

        public void Send(TMessage message)
        {
            lock (this.sendLock)
            {
                if (this.ConnectionState != ConnectionState.Activated)
                {
                    throw new InvalidOperationException($"Invalid state; {ConnectionState.Activated} expected.");
                }

                this.SendOverride(message);
            }
        }

        public async Task SendAsync(TMessage message)
        {
            await Task.Run(() => Monitor.Wait(this.sendLock));
            try
            {
                if (this.ConnectionState != ConnectionState.Activated)
                {
                    throw new InvalidOperationException($"Invalid state; {ConnectionState.Activated} expected.");
                }

                await this.SendAsyncOverride(message);
            }
            finally
            {
                Monitor.Exit(this.sendLock);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (this.ConnectionState != ConnectionState.Disposed &&
                this.ConnectionState != ConnectionState.Disposing)
            {
                this.ConnectionState = ConnectionState.Disposing;

                if (disposing)
                {
                    this.DisposeManaged();
                }

                this.DisposeUnmanaged();

                this.ConnectionState = ConnectionState.Disposed;
            }
        }

        protected virtual void DisposeManaged()
        {
        }

        protected virtual void DisposeUnmanaged()
        {
        }

        protected virtual void OnConnectionStateChanged()
            => this.ConnectionStateChanged?.Invoke(this, EventArgs.Empty);

        protected virtual void OnDisconnected()
            => this.ConnectionStateChanged?.Invoke(this, EventArgs.Empty);

        protected virtual void OnMessageReceived(TMessage message)
            => this.MessageReceived?.Invoke(this, new MessageReceivedEventArgs<TMessage>(message));

        protected abstract void ActivateOverride();

        protected abstract void SendOverride(TMessage message);

        protected abstract Task SendAsyncOverride(TMessage message);

        protected abstract void DisconnectOverride();

        protected abstract Task DisconnectAsyncOverride();
    }
}
