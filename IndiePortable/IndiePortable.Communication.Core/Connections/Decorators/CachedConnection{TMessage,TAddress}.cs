// <copyright file="CachedConnection{TMessage,TAddress}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core.Devices.Connections.Decorators
{
    using System;
    using System.Threading.Tasks;

    public class CachedConnection<TMessage, TAddress>
        : MessageConnectionBase<TMessage, TAddress>
        where TMessage : class
    {
        private readonly MessageConnectionBase<TMessage, TAddress> decoratedConnection;
        private readonly Guid signature = Guid.NewGuid();

        public CachedConnection(MessageConnectionBase<TMessage, TAddress> decoratedConnection)
        {
            this.decoratedConnection =
                decoratedConnection ?? throw new ArgumentNullException(nameof(decoratedConnection));

            this.Dispatcher = new MessageDispatcher<TMessage>(this.signature);
        }

        public override TAddress RemoteAddress => this.decoratedConnection.RemoteAddress;

        public MessageDispatcher<TMessage> Dispatcher { get; }

        protected override void ActivateOverride()
        {
            if (this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            this.decoratedConnection.Activate();
        }

        protected override void SendOverride(TMessage message)
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            this.decoratedConnection.Send(message);
        }

        protected override async Task SendAsyncOverride(TMessage message)
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            await this.decoratedConnection.SendAsync(message);
        }

        protected override void DisconnectOverride()
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            this.decoratedConnection.Disconnect();
        }

        protected override async Task DisconnectAsyncOverride()
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            await this.decoratedConnection.DisconnectAsync();
        }

        protected override void OnMessageReceived(TMessage message)
        {
            this.Dispatcher.PushMessage(message, this.signature);
            base.OnMessageReceived(message);
        }
    }
}
