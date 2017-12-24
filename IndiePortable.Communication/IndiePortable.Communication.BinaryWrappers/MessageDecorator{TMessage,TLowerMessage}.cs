// <copyright file="MessageDecorator{TMessage,TLowerMessage}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using IndiePortable.Communication.Message;

namespace IndiePortable.Communication.BinaryWrappers
{

    public abstract class MessageDecorator<TMessage, TLowerMessage>
        : DuplexMessageConnection<TMessage>
        where TMessage : class
        where TLowerMessage : class
    {
        protected MessageDecorator(DuplexMessageConnection<TLowerMessage> decoratedConnection)
        {
            this.DecoratedConnection = decoratedConnection ?? throw new ArgumentNullException(nameof(decoratedConnection));
        }

        public DuplexMessageConnection<TLowerMessage> DecoratedConnection { get; }

        /// <inheritdoc/>
        protected override void ActivateOverride()
        {
            this.DecoratedConnection.Activate();
        }

        /// <inheritdoc/>
        protected override void DisconnectOverride()
        {
            this.DecoratedConnection.Disconnect();
        }

        /// <inheritdoc/>
        protected override async Task DisconnectAsyncOverride()
        {
            await this.DecoratedConnection.DisconnectAsync();
        }

        /// <inheritdoc/>
        protected override void DisposeUnmanaged()
        {
            this.DecoratedConnection.Dispose();
        }
    }
}
