// <copyright file="ConnectionDecorator{TMessage,TAddress}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core.Devices.Connections.Decorators
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class ConnectionDecorator<TMessage, TAddress>
        : MessageConnectionBase<TMessage, TAddress>
        where TMessage : class
    {
        protected ConnectionDecorator(MessageConnectionBase<TMessage, TAddress> decoratedConnectoin)
        {
            this.DecoratedConnection =
                decoratedConnectoin ?? throw new ArgumentNullException(nameof(decoratedConnectoin));

            this.DecoratedConnection.Disconnected += (s, e) => this.OnDisconnected();
            this.DecoratedConnection.ConnectionStateChanged += (s, e) => this.OnConnectionStateChanged();
            this.DecoratedConnection.MessageReceived += (s, e) => this.OnMessageReceived(e.ReceivedMessage);
        }

        public override TAddress RemoteAddress => this.DecoratedConnection.RemoteAddress;

        protected MessageConnectionBase<TMessage, TAddress> DecoratedConnection { get; }

        protected override void ActivateOverride()
        {
            if (this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            this.DecoratedConnection.Activate();
        }

        protected override void SendOverride(TMessage message)
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            this.DecoratedConnection.Send(message);
        }

        protected override async Task SendAsyncOverride(TMessage message)
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            await this.DecoratedConnection.SendAsync(message);
        }

        protected override void DisconnectOverride()
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            this.DecoratedConnection.Disconnect();
        }

        protected override async Task DisconnectAsyncOverride()
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            await this.DecoratedConnection.DisconnectAsync();
        }

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();
            this.DecoratedConnection.Dispose();
        }
    }
}
