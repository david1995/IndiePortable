// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpConnection.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contians the TcpConnection class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.NetClassic
{
    using System;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using Devices;
    using Formatter;
    using AdvancedTasks;
    using Messages;
    using System.IO;
    using System.Threading;
    using Devices.ConnectionMessages;
    using Tcp;


    public sealed partial class TcpConnection
        : IConnection<IPPortAddressInfo>, IDisposable
    {
        /// <summary>
        /// The backing field for the <see cref="Cache" /> property.
        /// </summary>
        private readonly MessageDispatcher cacheBacking;

        /// <summary>
        /// The backing field for the <see cref="RemoteAddress" /> property.
        /// </summary>
        private readonly IPPortAddressInfo remoteAddressBacking;


        private readonly TcpClient client;


        private readonly NetworkStream stream;


        private readonly SemaphoreSlim streamSemaphore = new SemaphoreSlim(1, 1);


        private readonly BinaryFormatter formatter = BinaryFormatter.CreateWithCoreSurrogates();


        private readonly ConnectionMessageDispatcher<IPPortAddressInfo> connectionCache;


        private readonly SemaphoreSlim activationStateSemaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Determines whether the <see cref="TcpConnection" /> is disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// The backing field for the <see cref="IsConnected" /> property.
        /// </summary>
        private bool isConnectedBacking;

        /// <summary>
        /// The backing field for the <see cref="IsActivated" /> property.
        /// </summary>
        private bool isActivatedBacking;


        private StateTask messageReaderTask;


        private DateTime lastKeepAlive;


        public TcpConnection(TcpClient client, IPPortAddressInfo remoteAddress)
        {
            if (object.ReferenceEquals(client, null))
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (object.ReferenceEquals(remoteAddress, null))
            {
                throw new ArgumentNullException(nameof(remoteAddress));
            }

            this.cacheBacking = new MessageDispatcher(this);
            this.connectionCache = new ConnectionMessageDispatcher<IPPortAddressInfo>(this);

            this.handlerContent = new ConnectionMessageHandler<ConnectionContentMessage>(this.HandleContent);
            this.handlerDisconnect = new ConnectionMessageHandler<ConnectionDisconnectRequest>(this.HandleDisconnect);

            this.connectionCache.AddMessageHandler(this.handlerContent);
            this.connectionCache.AddMessageHandler(this.handlerDisconnect);

            this.client = client;
            this.remoteAddressBacking = remoteAddress;
            this.isConnectedBacking = true;

            this.stream = this.client.GetStream();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TcpConnection"/> class.
        /// </summary>
        ~TcpConnection()
        {
            this.Dispose(false);
        }


        public event EventHandler<MessageReceivedEventArgs> MessageReceived;


        public event EventHandler<ConnectionMessageReceivedEventArgs> ConnectionMessageReceived;


        public MessageDispatcher Cache => this.cacheBacking;


        public bool IsConnected => this.isConnectedBacking;


        public bool IsActivated => this.isActivatedBacking;


        public IPPortAddressInfo RemoteAddress => this.remoteAddressBacking;


        public void Activate()
        {
            this.activationStateSemaphore.Wait();
            try
            {
                if (this.IsActivated)
                {
                    throw new InvalidOperationException();
                }

                this.messageReaderTask = new StateTask(this.MessageReader);
                this.isActivatedBacking = true;
            }
            finally
            {
                this.activationStateSemaphore.Release();
            }
        }


        public void Disconnect()
        {
            var rq = new ConnectionDisconnectRequest();
            this.SendConnectionMessage(rq);
            this.connectionCache.Wait<ConnectionDisconnectRequest, ConnectionDisconnectResponse>(rq);

            this.isConnectedBacking = false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        ///     <para>Implements <see cref="IDisposable.Dispose()" /> implicitly.</para>
        /// </remarks>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        public void SendMessage(MessageBase message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!this.IsActivated)
            {
                throw new InvalidOperationException($"The {nameof(TcpConnection)} must be activated in order to send a message.");
            }
            
            try
            {
                this.SendConnectionMessage(new ConnectionContentMessage(message));
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException)
            {
            }
        }
        
        // TODO: add messages for encryption and kind of CryptoManager class

        public async Task SendMessageAsync(MessageBase message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!this.IsActivated)
            {
                throw new InvalidOperationException($"The {nameof(TcpConnection)} must be activated in order to send a message.");
            }

            try
            {
                await this.SendConnectionMessageAsync(new ConnectionContentMessage(message));
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException)
            {
            }
        }


        private void SendConnectionMessage(ConnectionMessageBase message)
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException($"The {nameof(TcpConnection)} must be activated in order to send a message.");
            }

            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            using (var ms = new MemoryStream())
            {
                ms.Seek(sizeof(int), SeekOrigin.Begin);
                this.formatter.Serialize(ms, message);

                ms.Seek(0, SeekOrigin.Begin);
                var msgLengthBytes = BitConverter.GetBytes(ms.Length - sizeof(int));
                ms.Write(msgLengthBytes, 0, sizeof(int));
                ms.Seek(0, SeekOrigin.Begin);

                this.streamSemaphore.Wait();
                try
                {
                    ms.CopyTo(this.stream);
                    this.stream.Flush();
                }
                finally
                {
                    this.streamSemaphore.Release();
                }
            }
        }


        private async Task SendConnectionMessageAsync(ConnectionMessageBase message)
        {
            if (!this.IsActivated)
            {
                throw new InvalidOperationException($"The {nameof(TcpConnection)} must be activated in order to send a message.");
            }

            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            using (var ms = new MemoryStream())
            {
                ms.Seek(sizeof(int), SeekOrigin.Begin);
                await Task.Factory.StartNew(() => this.formatter.Serialize(ms, message));

                ms.Seek(0, SeekOrigin.Begin);
                var msgLengthBytes = BitConverter.GetBytes(ms.Length - sizeof(int));
                await ms.WriteAsync(msgLengthBytes, 0, sizeof(int));
                ms.Seek(0, SeekOrigin.Begin);

                this.streamSemaphore.Wait();
                try
                {
                    await ms.CopyToAsync(this.stream);
                    await this.stream.FlushAsync();
                }
                finally
                {
                    this.streamSemaphore.Release();
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="MessageReceived" /> event.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="MessageBase" /> that has been received.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="message" /> is <c>null</c>.</para>
        /// </exception>
        private void RaiseMessageReceived(MessageBase message)
            => this.MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));

        /// <summary>
        /// Raises the <see cref="ConnectionMessageReceived" /> event.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="ConnectionMessageBase" /> that has been received.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="message" /> is <c>null</c>.</para>
        /// </exception>
        private void RaiseConnectionMessageReceived(ConnectionMessageBase message)
            => this.ConnectionMessageReceived?.Invoke(this, new ConnectionMessageReceivedEventArgs(message));

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                }

                this.messageReaderTask?.Stop();

                this.activationStateSemaphore.Dispose();

                this.streamSemaphore.Wait();
                this.stream.Dispose();
                this.streamSemaphore.Dispose();

                this.cacheBacking.Dispose();
                this.client.Client.Close();
                this.client.Close();
                this.isDisposed = true;
            }
        }


        private async void MessageReader(ITaskConnection taskConnection)
        {
            if (object.ReferenceEquals(taskConnection, null))
            {
                throw new ArgumentNullException(nameof(taskConnection));
            }

            try
            {
                while (!taskConnection.MustFinish)
                {
                    if (this.client.Available >= sizeof(int))
                    {
                        // read the length of the message
                        var lengthBytes = new byte[sizeof(int)];
                        await this.stream.ReadAsync(lengthBytes, 0, sizeof(int));
                        var length = BitConverter.ToInt32(lengthBytes, 0);

                        // fill the buffer with actual message
                        var buffer = new byte[length];
                        var currentLength = 0;

                        while (currentLength < length)
                        {
                            currentLength += await this.stream.ReadAsync(buffer, currentLength, length - currentLength);
                        }

                        using (var bufferStream = new MemoryStream(buffer, false))
                        {
                            ConnectionMessageBase message;
                            if (this.formatter.TryDeserialize(bufferStream, out message))
                            {
                                this.RaiseConnectionMessageReceived(message);
                            }
                        }
                    }

                    await Task.Delay(5);
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception exc)
            {
                taskConnection.ThrowException(exc);
                return;
            }

            taskConnection.Return();
        }
    }
}
