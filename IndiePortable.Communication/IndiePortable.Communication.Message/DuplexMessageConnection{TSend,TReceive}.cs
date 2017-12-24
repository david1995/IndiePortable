// <copyright file="DuplexMessageConnection{TSend,TReceive}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using IndiePortable.Communication.Core;

namespace IndiePortable.Communication.Message
{
    public abstract class DuplexMessageConnection<TSend, TReceive>
        : ISendMessageConnection<TSend>, IReceiveMessageConnection<TReceive>
        where TSend : class
        where TReceive : class
    {
        private readonly object sendLock = new object();
        private ConnectionState connectionState;

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplexMessageConnection{TMessage}"/> class.
        /// </summary>
        protected DuplexMessageConnection()
        {
            this.ConnectionStateChanged += (_, state) =>
            {
                if (state == ConnectionState.Disconnected)
                {
                    this.OnDisconnected();
                }
            };
        }

        /// <summary>
        /// Occurs when the <see cref="DuplexMessageConnection{TMessage}" /> has been disconnected.
        /// </summary>
        public event Action<IConnection> Disconnected;
        public event Action<IConnection, ConnectionState> ConnectionStateChanged;
        public event Action<IReceiveMessageConnection<TReceive>, TReceive> MessageReceived;

        public ConnectionState ConnectionState
        {
            get => this.connectionState;
            protected set
            {
                this.connectionState = value;
                this.OnConnectionStateChanged(value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="DuplexMessageConnection{TMessage}" /> is connected to the other end.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="DuplexMessageConnection{TMessage}" /> is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected => this.ConnectionState != ConnectionState.Lost
                                && this.connectionState != ConnectionState.Disconnecting
                                && this.ConnectionState != ConnectionState.Disconnected
                                && this.ConnectionState != ConnectionState.Disposing
                                && this.ConnectionState != ConnectionState.Disposed;

        /// <summary>
        /// Gets a value indicating whether the <see cref="DuplexMessageConnection{TMessage}" /> is activated.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="DuplexMessageConnection{TMessage}" /> is activated; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// <para>
        /// Messages can only be sent or received if <see cref="IsActivated" /> is <c>true</c>.
        /// Otherwise, an <see cref="InvalidOperationException" /> will be thrown.
        /// </para>
        /// </remarks>
        public bool IsActivated => this.ConnectionState == ConnectionState.Activated;

        public void StartInit()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Activates the <see cref="DuplexMessageConnection{TMessage}" />.
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

        public void Send(TSend message)
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

        public async Task SendAsync(TSend message)
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
            if (this.ConnectionState == ConnectionState.Disposed || this.ConnectionState == ConnectionState.Disposing)
            {
                return;
            }

            this.ConnectionState = ConnectionState.Disposing;

            if (disposing)
            {
                this.DisposeManaged();
            }

            this.DisposeUnmanaged();

            this.ConnectionState = ConnectionState.Disposed;
        }

        protected virtual void DisposeManaged()
        {
        }

        protected virtual void DisposeUnmanaged()
        {
            this.ConnectionStateChanged = null;
            this.Disconnected = null;
            this.MessageReceived = null;
        }

        protected virtual void OnConnectionStateChanged(ConnectionState newState)
            => this.ConnectionStateChanged?.Invoke(this, newState);

        protected virtual void OnDisconnected()
            => this.Disconnected?.Invoke(this);

        protected virtual void OnMessageReceived(TReceive message)
            => this.MessageReceived?.Invoke(this, message);

        protected abstract void ActivateOverride();

        protected abstract void SendOverride(TSend message);

        protected abstract Task SendAsyncOverride(TSend message);

        protected abstract void DisconnectOverride();

        protected abstract Task DisconnectAsyncOverride();
    }
}
