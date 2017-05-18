// <copyright file="KeepAliveConnection{TAddress}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core.Devices.Connections.Decorators
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using IndiePortable.AdvancedTasks;
    using IndiePortable.Communication.Core.Messages;

    public class KeepAliveConnection<TAddress>
        : ConnectionDecorator<MessageBase, TAddress>
    {
        private AutoResetEvent keepAliveEvent;
        private StateTask keepAliveSender;

        public KeepAliveConnection(
            ConnectionBase<MessageBase, TAddress> decoratedConnection,
            TimeSpan keepAliveFrequency)
            : base(decoratedConnection)
        {
            this.KeepAliveFrequency = keepAliveFrequency <= TimeSpan.Zero
                                    ? throw new ArgumentOutOfRangeException(nameof(keepAliveFrequency))
                                    : keepAliveFrequency;

            this.DecoratedConnection.Disconnected += (s, e) => this.OnDisconnected();
            this.DecoratedConnection.ConnectionStateChanged += (s, e) => this.OnConnectionStateChanged();
            this.DecoratedConnection.MessageReceived += (s, e) => this.OnMessageReceived(e.ReceivedMessage);

            this.keepAliveSender = new StateTask(this.SendKeepAlives, false);

            this.Disconnected += (s, e) =>
            {
                this.keepAliveSender.Stop();
            };
        }

        public TimeSpan KeepAliveFrequency { get; }

        public override TAddress RemoteAddress => this.DecoratedConnection.RemoteAddress;

        protected override void ActivateOverride()
        {
            if (this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            base.ActivateOverride();
            this.keepAliveSender.Start();
        }

        protected override void DisconnectOverride()
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            this.keepAliveSender.StopAndAwait();
            base.DisconnectOverride();
        }

        protected override async Task DisconnectAsyncOverride()
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            await this.keepAliveSender.StopAndAwaitAsync();
            await base.DisconnectAsyncOverride();
        }

        protected override void DisposeUnmanaged()
        {
            this.keepAliveSender.Stop();
            base.DisposeUnmanaged();
        }

        private async void SendKeepAlives(ITaskConnection conn)
        {
            while (!conn.MustFinish)
            {
                using (var ct = new CancellationTokenSource())
                {
                    void HandleStopRequested(object s, EventArgs e) => ct.Cancel();

                    conn.StopRequested += HandleStopRequested;
                    if (!await Task.Run(() => this.keepAliveEvent.WaitOne(), ct.Token))
                    {
                        this.ConnectionState = ConnectionState.Disconnected;
                    }

                    conn.StopRequested -= HandleStopRequested;
                }
            }
        }
    }
}
