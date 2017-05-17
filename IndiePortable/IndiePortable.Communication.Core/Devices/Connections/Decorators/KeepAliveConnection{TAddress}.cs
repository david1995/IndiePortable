// <copyright file="KeepAliveConnection{TAddress}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core.Devices.Connections.Decorators
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using IndiePortable.AdvancedTasks;
    using IndiePortable.Communication.Core.Messages;

    public class KeepAliveConnection<TAddress>
        : ConnectionBase<MessageBase, TAddress>
    {

        private ConnectionBase<MessageBase, TAddress> decoratedConnection;

        private AutoResetEvent keepAliveEvent;

        private StateTask keepAliveSender;

        public KeepAliveConnection(
            ConnectionBase<MessageBase, TAddress> decoratedConnection,
            TimeSpan keepAliveFrequency)
        {
            this.decoratedConnection =
                decoratedConnection ?? throw new ArgumentNullException(nameof(decoratedConnection));

            this.KeepAliveFrequency = keepAliveFrequency <= TimeSpan.Zero
                                    ? throw new ArgumentOutOfRangeException(nameof(keepAliveFrequency))
                                    : keepAliveFrequency;

            this.decoratedConnection.Disconnected += (s, e) => this.OnDisconnected();
            this.decoratedConnection.ConnectionStateChanged += (s, e) => this.OnConnectionStateChanged();
            this.decoratedConnection.MessageReceived += (s, e) => this.OnMessageReceived(e.ReceivedMessage);

            this.Disconnected += (s, e) =>
            {
                this.keepAliveSender.Stop();
            };
        }

        public TimeSpan KeepAliveFrequency { get; }

        public override TAddress RemoteAddress => this.decoratedConnection.RemoteAddress;

        protected override void ActivateOverride()
        {
            if (this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            this.decoratedConnection.Activate();
        }

        protected override void SendOverride(MessageBase message)
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            this.decoratedConnection.Send(message);
        }

        protected override async Task SendAsyncOverride(MessageBase message)
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

            this.keepAliveSender.Stop();
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

        protected override void DisposeUnmanaged()
        {
            this.decoratedConnection.Dispose();
            this.keepAliveSender.Stop();
            base.DisposeUnmanaged();
        }

        private async void SendKeepAlives(ITaskConnection conn)
        {
            // TODO: add StopRequested event to ITaskConnection interface
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
