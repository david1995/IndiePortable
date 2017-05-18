// <copyright file="ConnectionConverterDecorator{TMessageLower,TMessageUpper,TAddress}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core.Devices.Connections.Decorators
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public class ConnectionConverterDecorator<TMessageLower, TMessageUpper, TAddress>
        : ConnectionBase<TMessageUpper, TAddress>
        where TMessageLower : class
        where TMessageUpper : class
    {
        private readonly Func<TMessageLower, TMessageUpper> upstreamConverter;
        private readonly Func<TMessageUpper, TMessageLower> downstreamConverter;

        protected ConnectionConverterDecorator(
            ConnectionBase<TMessageLower, TAddress> decoratedConnection,
            Func<TMessageLower, TMessageUpper> upstreamConverter,
            Func<TMessageUpper, TMessageLower> downstreamConverter)
        {
            this.DecoratedConnection =
                decoratedConnection ?? throw new ArgumentNullException(nameof(decoratedConnection));
            this.upstreamConverter =
                upstreamConverter ?? throw new ArgumentNullException(nameof(upstreamConverter));
            this.downstreamConverter =
                downstreamConverter ?? throw new ArgumentNullException(nameof(downstreamConverter));
        }

        public override TAddress RemoteAddress => this.DecoratedConnection.RemoteAddress;

        protected ConnectionBase<TMessageLower, TAddress> DecoratedConnection { get; }

        protected override void ActivateOverride()
        {
            if (this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            this.DecoratedConnection.Activate();
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

        protected override void SendOverride(TMessageUpper message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var conv = this.downstreamConverter(message);
        }

        protected override async Task SendAsyncOverride(TMessageUpper message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var conv = await Task.Factory.StartNew(() => this.downstreamConverter(message));
        }

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();
            this.DecoratedConnection.Dispose();
        }
    }
}
