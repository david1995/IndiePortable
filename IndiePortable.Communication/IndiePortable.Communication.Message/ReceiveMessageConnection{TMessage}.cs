// <copyright file="ReceiveMessageConnection{TMessage}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using IndiePortable.Communication.Core;

namespace IndiePortable.Communication.Message
{
    public abstract class ReceiveMessageConnection<TMessage>
        : IReceiveMessageConnection<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// The backing field for the <see cref="ConnectionState"/> property.
        /// </summary>
        private ConnectionState connectionState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiveMessageConnection{TMessage}"/> class.
        /// </summary>
        protected ReceiveMessageConnection()
        {
            this.ConnectionStateChanged += (_, state) =>
            {
                if (state == ConnectionState.Disconnected)
                {
                    this.OnDisconnected();
                }
            };
        }

        public event Action<IConnection, ConnectionState> ConnectionStateChanged;
        public event Action<IConnection> Disconnected;
        public event Action<IReceiveMessageConnection<TMessage>, TMessage> MessageReceived;

        public ConnectionState ConnectionState
        {
            get => this.connectionState;
            protected set
            {
                this.connectionState = value;
                this.OnConnectionStateChanged(value);
            }
        }

        public void StartInit()
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

            await Task.Factory.StartNew(this.DisconnectOverride);

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

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        /// <exception cref="InvalidOperationException">Disposing not possible.</exception>
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

        protected virtual void OnConnectionStateChanged(ConnectionState newState)
            => this.ConnectionStateChanged?.Invoke(this, newState);

        protected virtual void OnDisconnected()
            => this.Disconnected?.Invoke(this);

        protected virtual void ActivateOverride()
        {
        }

        protected virtual void InitializeOverride()
        {
        }

        protected abstract void DisconnectOverride();

        protected abstract void DisposeUnmanaged();

        protected abstract void DisposeManaged();
    }
}
