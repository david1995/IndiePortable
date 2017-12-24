// <copyright file="StreamDecorator.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using IndiePortable.Communication.Binary;
using IndiePortable.Communication.Core;

namespace IndiePortable.Communication.BinaryWrappers
{
    public abstract class StreamDecorator
        : StreamConnection
    {
        protected StreamDecorator(StreamConnection decoratedConnection)
        {
            this.DecoratedConnection = decoratedConnection is null
                                     ? throw new ArgumentNullException(nameof(decoratedConnection))
                                     : !decoratedConnection.IsConnected
                                     ? throw new ArgumentException("The deocrated connection must be connected.", nameof(decoratedConnection))
                                     : decoratedConnection;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="StreamDecorator"/> class.
        /// </summary>
        ~StreamDecorator()
        {
            this.DisposeDecorator(false);
        }

        public StreamConnection DecoratedConnection { get; }

        public virtual void Disconnect(bool decoratorOnly)
        {
            this.Disconnect();

            if (!decoratorOnly)
            {
                this.DecoratedConnection.Disconnect();
            }
        }

        public virtual async Task DisconnectAsyncOverride(bool decoratorOnly)
        {
            await this.DisconnectAsync();

            if (!decoratorOnly)
            {
                await this.DecoratedConnection.DisconnectAsync();
            }
        }

        public void DisposeDecorator()
        {
            this.DisposeDecorator(true);
            GC.SuppressFinalize(this);
        }

        protected override void ActivateOverride()
        {
        }

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();
            this.DecoratedConnection.Dispose();
        }

        protected void DisposeDecorator(bool disposing)
        {
            if (this.ConnectionState == ConnectionState.Disposing ||
                this.ConnectionState == ConnectionState.Disposed)
            {
                return;
            }

            if (disposing)
            {
                this.DisposeDecoratorManaged();
            }

            this.DisposeDecoratorUnmanaged();
        }

        protected virtual void DisposeDecoratorManaged()
        {
        }

        protected virtual void DisposeDecoratorUnmanaged()
        {
        }
    }
}
