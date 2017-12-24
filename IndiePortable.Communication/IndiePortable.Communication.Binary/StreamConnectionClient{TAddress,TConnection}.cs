// <copyright file="StreamConnectionClient{TAddress,TConnection}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using IndiePortable.Communication.Core;

namespace IndiePortable.Communication.Binary
{
    /// <inheritdoc />
    /// <summary>
    /// Represents the local end point of a stream connection.
    /// This class is abstract.
    /// </summary>
    /// <typeparam name="TAddress">The type of the address.</typeparam>
    /// <typeparam name="TConnection">
    /// The type of the connection.
    /// Must derive from <see cref="StreamConnection" />.
    /// </typeparam>
    /// <seealso cref="IStreamConnectionClient{TConnection, TAddress}" />
    public abstract class StreamConnectionClient<TConnection, TAddress>
        : IStreamConnectionClient<TConnection, TAddress>
        where TConnection : IStreamConnection
    {
        /// <summary>
        /// The backing field for the <see cref="ClientState"/> property.
        /// </summary>
        private ConnectionClientState clientState;

        /// <summary>
        /// Occurs when the value of the <see cref="ClientState"/> property has been changed.
        /// </summary>
        public event Action<IStreamConnectionClient<TConnection, TAddress>> ClientStateChanged;

        public event Action<IStreamConnectionClient<TConnection, TAddress>> Disconnected;

        public TConnection Connection { get; private set; }

        /// <summary>
        /// Gets the state of the client.
        /// </summary>
        /// <value>
        /// The state of the client.
        /// </value>
        public ConnectionClientState ClientState
        {
            get => this.clientState;
            private set
            {
                this.clientState = value;
                this.OnClientStateChanged();
            }
        }

        public abstract TAddress Address { get; }

        /// <summary>
        /// Initializes the <see cref="StreamConnectionClient{TAddress, TConnection}"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <list type="bullet">
        /// <item>The state is not <see cref="ConnectionClientState.Constructed"/> before starting the initialization.</item>
        /// <item>The state is not <see cref="ConnectionClientState.Initializing"/> after running the initialization logic.</item>
        /// </list>
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method wraps a call to the custom initialization implementation inside the method <see cref="InitializeOverride()"/>.
        /// </para>
        /// <para>
        /// Before initialization, the <see cref="ClientState"/> property must be <see cref="ConnectionClientState.Constructed"/>.
        /// </para>
        /// <para>
        /// This method sets the value of the <see cref="ClientState"/> property to <see cref="ConnectionClientState.Initialized"/>
        /// after running the custom initialization logic.
        /// </para>
        /// </remarks>
        public void Initialize()
        {
            this.ClientState = this.ClientState != ConnectionClientState.Constructed
                             ? throw new InvalidOperationException($"The state is invalid. {ConnectionClientState.Constructed} expected.")
                             : ConnectionClientState.Initializing;

            this.InitializeOverride();

            this.ClientState = this.ClientState != ConnectionClientState.Initializing
                             ? throw new InvalidOperationException($"The state is invalid. {ConnectionClientState.Initializing} expected.")
                             : ConnectionClientState.Initialized;
        }

        public void ConnectTo(TAddress address)
        {
            this.ClientState = this.ClientState != ConnectionClientState.Initialized &&
                               this.ClientState != ConnectionClientState.ConnectionLost &&
                               this.ClientState != ConnectionClientState.Disconnected
                             ? throw new InvalidOperationException("The state is invalid.")
                             : ConnectionClientState.Connecting;

            try
            {
                var (succ, ret) = this.ConnectOverride(address);
                if (!succ)
                {
                    throw new InvalidOperationException();
                }

                this.Connection = ret;

                this.ClientState = this.ClientState != ConnectionClientState.Connecting
                                 ? throw new InvalidOperationException("The state is invalid.")
                                 : ConnectionClientState.Connected;
            }
            catch
            {
                this.ClientState = ConnectionClientState.ConnectionLost;
                throw;
            }
        }

        public bool TryConnectTo(TAddress address)
        {
            this.ClientState = this.ClientState != ConnectionClientState.Initialized
                               && this.ClientState != ConnectionClientState.ConnectionLost
                               && this.ClientState != ConnectionClientState.Disconnected
                               ? throw new InvalidOperationException("The state is invalid.")
                               : ConnectionClientState.Connecting;

            try
            {
                var (succ, ret) = this.ConnectOverride(address);
                if (!succ)
                {
                    return false;
                }

                this.Connection = ret;

                this.ClientState = this.ClientState != ConnectionClientState.Connecting
                                 ? throw new InvalidOperationException("The state is invalid.")
                                 : ConnectionClientState.Connected;

                return true;
            }
            catch
            {
                this.ClientState = ConnectionClientState.ConnectionLost;
                throw;
            }
        }

        public void Disconnect()
        {
            this.ClientState = this.ClientState != ConnectionClientState.Connected
                             ? throw new InvalidOperationException()
                             : ConnectionClientState.Disconnecting;

            try
            {
                this.DisconnectOverride();

                this.OnDisconnected();

                this.ClientState = this.ClientState != ConnectionClientState.Disconnecting
                                 ? throw new InvalidOperationException()
                                 : ConnectionClientState.Disconnected;
            }
            catch
            {
                this.ClientState = ConnectionClientState.ConnectionLost;
                throw;
            }
        }

        public async Task DisconnectAsync()
        {
            this.ClientState = this.ClientState != ConnectionClientState.Connected
                ? throw new InvalidOperationException()
                : ConnectionClientState.Disconnecting;

            try
            {
                await this.DisconnectOverrideAsync();

                this.OnDisconnected();

                this.ClientState = this.ClientState != ConnectionClientState.Disconnecting
                    ? throw new InvalidOperationException()
                    : ConnectionClientState.Disconnected;
            }
            catch
            {
                this.ClientState = ConnectionClientState.ConnectionLost;
                throw;
            }
        }

        /// <inheritdoc />
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
        /// <exception cref="InvalidOperationException">The state is invalid.</exception> TODO: improve exception description
        protected void Dispose(bool disposing)
        {
            switch (this.ClientState)
            {
                case ConnectionClientState.Initializing:
                case ConnectionClientState.Initialized:
                case ConnectionClientState.Disconnected:
                case ConnectionClientState.ConnectionLost:
                    if (disposing)
                    {
                        this.DisposeManaged();
                    }

                    this.DisposeUnmanaged();
                    break;

                case ConnectionClientState.Disposed: break;

                default: throw new InvalidOperationException("The state is invalid.");
            }
        }

        /// <summary>
        /// Called when the value of the <see cref="ClientState"/> property has been changed.
        /// </summary>
        protected virtual void OnClientStateChanged() => this.ClientStateChanged?.Invoke(this);

        protected virtual void OnDisconnected() => this.Disconnected?.Invoke(this);

        /// <summary>
        /// When overriden in a derived class, releases managed objects used by the current object.
        /// </summary>
        protected virtual void DisposeManaged()
        {
        }

        /// <summary>
        /// When overriden in a derived class, releases unmanaged memory used by the current object.
        /// </summary>
        protected virtual void DisposeUnmanaged()
        {
        }

        protected abstract void DisconnectOverride();

        protected virtual async Task DisconnectOverrideAsync()
            => await Task.Factory.StartNew(this.DisconnectOverride);

        /// <summary>
        /// When overriden in a derived class, executes implementation-specific initialization logic.
        /// </summary>
        protected abstract void InitializeOverride();

        protected abstract (bool Success, TConnection Result) ConnectOverride(TAddress address);
    }
}
