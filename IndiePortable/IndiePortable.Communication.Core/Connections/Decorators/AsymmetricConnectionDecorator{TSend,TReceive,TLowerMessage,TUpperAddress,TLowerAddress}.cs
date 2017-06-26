// <copyright file="AsymmetricConnectionDecorator{TSend,TReceive,TLowerMessage,TUpperAddress,TLowerAddress}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core.Devices.Connections.Decorators
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The base class for symmetric-to-asymmetric connection decorators.
    /// </summary>
    /// <typeparam name="TSend">The type of the sent messages.</typeparam>
    /// <typeparam name="TReceive">The type of the received messages.</typeparam>
    /// <typeparam name="TLowerMessage">The type of the lower-layer messages.</typeparam>
    /// <typeparam name="TUpperAddress">The type of the upper-layer address.</typeparam>
    /// <typeparam name="TLowerAddress">The type of the lower-level address.</typeparam>
    public abstract class AsymmetricConnectionDecorator<TSend, TReceive, TLowerMessage, TUpperAddress, TLowerAddress>
        : AsymmetricConnectionBase<TSend, TReceive, TUpperAddress>
        where TSend : class
        where TReceive : class
        where TLowerMessage : class
    {
        private readonly Func<TSend, TLowerMessage> downstreamConverter;
        private readonly Func<TLowerMessage, TReceive> upstreamConverter;

        protected AsymmetricConnectionDecorator(
            MessageConnectionBase<TLowerMessage, TLowerAddress> decoratedConnection,
            Func<TSend, TLowerMessage> downstreamConverter,
            Func<TLowerMessage, TReceive> upstreamConverter)
        {
            this.DecoratedConnection = decoratedConnection ?? throw new ArgumentNullException(nameof(decoratedConnection));
            this.downstreamConverter = downstreamConverter ?? throw new ArgumentNullException(nameof(downstreamConverter));
            this.upstreamConverter = upstreamConverter ?? throw new ArgumentNullException(nameof(upstreamConverter));

            this.DecoratedConnection.ConnectionStateChanged += (s, e) => this.ConnectionState = this.DecoratedConnection.ConnectionState;
            this.DecoratedConnection.Disconnected += (s, e) => this.OnDisconnected();
            this.DecoratedConnection.MessageReceived += (s, e) => this.OnMessageReceived(this.upstreamConverter(e.ReceivedMessage));
        }

        protected MessageConnectionBase<TLowerMessage, TLowerAddress> DecoratedConnection { get; }

        protected override void ActivateOverride()
        {
            this.DecoratedConnection.Activate();
        }

        protected override void SendOverride(TSend message)
        {
            var lower = this.downstreamConverter(message);
            this.DecoratedConnection.Send(lower);
        }

        protected override async Task SendAsyncOverride(TSend message)
        {
            var lower = this.downstreamConverter(message);
            await this.DecoratedConnection.SendAsync(lower);
        }

        protected override void DisconnectOverride()
        {
            this.DecoratedConnection.Disconnect();
        }

        protected override async Task DisconnectAsyncOverride()
        {
            await this.DecoratedConnection.DisconnectAsync();
        }
    }
}
