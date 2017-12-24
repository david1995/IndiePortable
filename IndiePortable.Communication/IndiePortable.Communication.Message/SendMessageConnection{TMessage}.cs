// <copyright file="SendMessageConnection{TMessage}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using IndiePortable.Communication.Core;

namespace IndiePortable.Communication.Message
{
    public abstract class SendMessageConnection<TMessage>
        : IDisposable
    {
        private readonly object sendLock = new object();

        /// <summary>
        /// The backing field for the <see cref="ConnectionState"/> property.
        /// </summary>
        private ConnectionState connectionState;

        protected SendMessageConnection()
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
        /// Occurs when the value <see cref="ConnectionState"/> property has been changed.
        /// </summary>
        public event EventHandler ConnectionStateChanged;

        public event EventHandler Disconnected;

        public ConnectionState ConnectionState
        {
            get => this.connectionState;
            protected set
            {
                this.connectionState = value;
                this.OnConnectionStateChanged();
            }
        }

        public void Initialize()
        {
            this.ConnectionState = this.ConnectionState != ConnectionState.Constructed
                                 ? throw new InvalidOperationException("The state is invalid.")
                                 : ConnectionState.Initializing;

            this.InitializeOverride();

            this.ConnectionState = this.ConnectionState != ConnectionState.Initializing
                                 ? throw new InvalidOperationException("The state is invalid.")
                                 : ConnectionState.Initialized;
        }

        public void Activate()
        {
            this.ConnectionState = this.ConnectionState != ConnectionState.Initialized
                                 ? throw new InvalidOperationException("The state is invalid.")
                                 : ConnectionState.Activating;

            this.ActivateOverride();

            this.ConnectionState = this.ConnectionState != ConnectionState.Activating
                                 ? throw new InvalidOperationException("The state is invalid.")
                                 : ConnectionState.Activated;
        }

        public void Send(TMessage message)
        {
            lock (this.sendLock)
            {
                this.SendOverride(message);
            }
        }

        public async Task SendAsync(TMessage message)
        {
            Monitor.TryEnter(this.sendLock, Timeout.InfiniteTimeSpan);
            await this.SendAsyncOverride(message);
            Monitor.Exit(this.sendLock);
        }

        public void Disconnect()
        {
            this.ConnectionState = this.ConnectionState != ConnectionState.Activated
                                 ? throw new InvalidOperationException("The state is invalid.")
                                 : ConnectionState.Disconnecting;

            this.DisconnectOverride();

            this.connectionState = this.ConnectionState != ConnectionState.Disconnecting
                                 ? throw new InvalidOperationException("The state is invalid.")
                                 : ConnectionState.Disconnected;
        }

        public async Task DisconnectAsync()
        {
            this.ConnectionState = this.ConnectionState != ConnectionState.Activated
                                 ? throw new InvalidOperationException("The state is invalid.")
                                 : ConnectionState.Disconnecting;

            await Task.Factory.StartNew(() => this.DisconnectOverride());

            this.connectionState = this.ConnectionState != ConnectionState.Disconnecting
                                 ? throw new InvalidOperationException("The state is invalid.")
                                 : ConnectionState.Disconnected;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(false);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            switch (this.ConnectionState)
            {
                case ConnectionState.Lost:
                case ConnectionState.Disconnected:
                    this.ConnectionState = ConnectionState.Disposing;

                    if (disposing)
                    {
                        this.DisposeManaged();
                    }

                    this.DisposeUnmanaged();

                    this.ConnectionState = ConnectionState.Disposed;
                    break;

                case ConnectionState.Disposed:
                case ConnectionState.Disposing: break;
                default: throw new InvalidOperationException("Disposing not possible.");
            }
        }

        protected virtual void OnConnectionStateChanged() => this.ConnectionStateChanged?.Invoke(this, EventArgs.Empty);

        protected virtual void OnDisconnected() => this.Disconnected?.Invoke(this, EventArgs.Empty);

        protected virtual void ActivateOverride()
        {
        }

        protected virtual void InitializeOverride()
        {
        }

        protected abstract void DisconnectOverride();

        protected abstract void DisposeUnmanaged();

        protected abstract void DisposeManaged();

        protected abstract void SendOverride(TMessage message);

        protected abstract Task SendAsyncOverride(TMessage message);
    }
}
