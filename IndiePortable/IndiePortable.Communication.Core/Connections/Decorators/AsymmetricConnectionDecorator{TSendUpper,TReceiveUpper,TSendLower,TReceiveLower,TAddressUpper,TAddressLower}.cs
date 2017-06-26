// <copyright file="AsymmetricConnectionDecorator{TSendUpper,TReceiveUpper,TSendLower,TReceiveLower,TAddressUpper,TAddressLower}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core.Devices.Connections.Decorators
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The base class for asymmetric-to-asymmetric connection decorators.
    /// </summary>
    /// <typeparam name="TSendUpper">The type of the upper-layer sent messages.</typeparam>
    /// <typeparam name="TReceiveUpper">The type of the upper-layer received messages.</typeparam>
    /// <typeparam name="TSendLower">The type of the lower-layer sent messages.</typeparam>
    /// <typeparam name="TReceiveLower">The type of the lower-level received messages.</typeparam>
    /// <typeparam name="TAddressUpper">The type of the upper-level address.</typeparam>
    /// <typeparam name="TAddressLower">The type of the lower-level address.</typeparam>
    /// <seealso cref="Connections.AsymmetricConnectionBase{TSend, TReceive, TAddress}" />
    public abstract class AsymmetricConnectionDecorator<TSendUpper, TReceiveUpper, TSendLower, TReceiveLower, TAddressUpper, TAddressLower>
        : AsymmetricConnectionBase<TSendUpper, TReceiveUpper, TAddressUpper>
        where TSendUpper : class
        where TReceiveUpper : class
        where TSendLower : class
        where TReceiveLower : class
    {
        private readonly Func<TSendUpper, TSendLower> downstreamConverter;
        private readonly Func<TReceiveLower, TReceiveUpper> upstreamConverter;

        protected AsymmetricConnectionDecorator(
            AsymmetricConnectionBase<TSendLower, TReceiveLower, TAddressLower> decoratedConnection,
            Func<TSendUpper, TSendLower> downstreamConverter,
            Func<TReceiveLower, TReceiveUpper> upstreamConverter)
        {
            this.DecoratedConnection = decoratedConnection ?? throw new ArgumentNullException(nameof(decoratedConnection));
            this.downstreamConverter = downstreamConverter ?? throw new ArgumentNullException(nameof(downstreamConverter));
            this.upstreamConverter = upstreamConverter ?? throw new ArgumentNullException(nameof(upstreamConverter));

            this.DecoratedConnection.ConnectionStateChanged += (s, e) => this.ConnectionState = this.DecoratedConnection.ConnectionState;
            this.DecoratedConnection.Disconnected += (s, e) => this.OnDisconnected();
            this.DecoratedConnection.MessageReceived += (s, e) => this.OnMessageReceived(this.upstreamConverter(e.ReceivedMessage));
        }

        protected AsymmetricConnectionBase<TSendLower, TReceiveLower, TAddressLower> DecoratedConnection { get; }

        protected override void ActivateOverride()
        {
            this.DecoratedConnection.Activate();
        }

        protected override void DisconnectOverride()
        {
            this.DecoratedConnection.Disconnect();
        }

        protected override async Task DisconnectAsyncOverride()
        {
            await this.DecoratedConnection.DisconnectAsync();
        }

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();
            this.DecoratedConnection.Dispose();
        }

        protected override void SendOverride(TSendUpper message)
        {
            var send = this.downstreamConverter(message);
            this.DecoratedConnection.Send(send);
        }

        protected override async Task SendAsyncOverride(TSendUpper message)
        {
            var send = this.downstreamConverter(message);
            await this.DecoratedConnection.SendAsync(send);
        }
    }
}
