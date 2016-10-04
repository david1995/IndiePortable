// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpConnection.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the TcpConnection class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.UniversalWindows
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using AdvancedTasks;
    using Devices;
    using Devices.ConnectionMessages;
    using Formatter;
    using Messages;
    using Tcp;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;
    using RTBuffer = Windows.Storage.Streams.Buffer;


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


        private readonly BinaryFormatter formatter = BinaryFormatter.CreateWithCoreSurrogates();


        private readonly ConnectionMessageDispatcher<IPPortAddressInfo> connectionCache;


        private readonly SemaphoreSlim writeSemaphore = new SemaphoreSlim(1, 1);


        private readonly StreamSocket socket;


        private readonly IInputStream inputStream;


        private readonly Stream outputStream;


        private readonly SemaphoreSlim activationStateSemaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// The backing field for the  <see cref="IsActivated" /> property.
        /// </summary>
        private bool isActivatedBacking;

        /// <summary>
        /// The backing field for the <see cref="IsConnected" /> property.
        /// </summary>
        private bool isConnectedBacking;

        /// <summary>
        /// Indicates whether the <see cref="TcpConnection" /> is disposed.
        /// </summary>
        private bool isDisposed;


        private StateTask messageReaderTask;


        private DateTime lastKeepAlive;


        public TcpConnection(StreamSocket socket, IPPortAddressInfo remoteAddress)
        {
            if (object.ReferenceEquals(socket, null))
            {
                throw new ArgumentNullException(nameof(socket));
            }

            if (object.ReferenceEquals(remoteAddress, null))
            {
                throw new ArgumentNullException(nameof(remoteAddress));
            }
            
            this.socket = socket;
            this.remoteAddressBacking = remoteAddress;
            this.inputStream = this.socket.InputStream;
            this.outputStream = this.socket.OutputStream.AsStreamForWrite();

            this.connectionCache = new ConnectionMessageDispatcher<IPPortAddressInfo>(this);

            this.handlerDisconnect = new ConnectionMessageHandler<ConnectionDisconnectRequest>(this.HandleDisconnect);
            this.handlerKeepAlive = new ConnectionMessageHandler<ConnectionMessageKeepAlive>(this.HandleKeepAlive);
            this.handlerContent = new ConnectionMessageHandler<ConnectionContentMessage>(this.HandleContent);

            this.connectionCache.AddMessageHandler(this.handlerDisconnect);
            this.connectionCache.AddMessageHandler(this.handlerKeepAlive);
            this.connectionCache.AddMessageHandler(this.handlerContent);

            this.isConnectedBacking = true;
            this.lastKeepAlive = DateTime.Now;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TcpConnection" /> class.
        /// </summary>
        ~TcpConnection()
        {
            this.Dispose(false);
        }


        public event EventHandler<ConnectionMessageReceivedEventArgs> ConnectionMessageReceived;

        /// <summary>
        /// Raised when a <see cref="MessageBase" /> object has been received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;


        public MessageDispatcher Cache => this.cacheBacking;


        public bool IsActivated => this.isActivatedBacking;


        public bool IsConnected => this.isConnectedBacking;


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
            if (!this.IsActivated || this.isDisposed || !this.IsConnected)
            {
                throw new InvalidOperationException();
            }

            var rq = new ConnectionDisconnectRequest();
            this.SendConnectionMessage(rq);
            this.connectionCache.Wait<ConnectionDisconnectRequest, ConnectionDisconnectResponse>(rq);
            this.isConnectedBacking = false;
            this.messageReaderTask.Stop();
            this.Dispose();
        }


        public void Disconnect(TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }

            if (!this.IsActivated || this.isDisposed || !this.IsConnected)
            {
                throw new InvalidOperationException();
            }

            var rq = new ConnectionDisconnectRequest();
            this.SendConnectionMessage(rq);
            this.connectionCache.Wait<ConnectionDisconnectRequest, ConnectionDisconnectResponse>(rq, timeout);
            this.isConnectedBacking = false;
            this.messageReaderTask.Stop();
            this.Dispose();
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

            this.SendConnectionMessage(new ConnectionContentMessage(message));
        }


        public async Task SendMessageAsync(MessageBase message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            await this.SendConnectionMessageAsync(new ConnectionContentMessage(message));
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

            using (var str = new MemoryStream())
            {
                // make place for length prefix
                str.Seek(sizeof(int), SeekOrigin.Begin);

                // serialize message into stream
                this.formatter.Serialize(str, message);
                
                // write length of message
                str.Seek(0, SeekOrigin.Begin);
                var lengthBytes = BitConverter.GetBytes((int)str.Length - sizeof(int));
                str.Write(lengthBytes, 0, sizeof(int));
                str.Seek(0, SeekOrigin.Begin);

                this.writeSemaphore.Wait();
                try
                {
                    str.CopyTo(this.outputStream);
                    this.outputStream.Flush();
                }
                finally
                {
                    this.writeSemaphore.Release();
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
            
            using (var str = new MemoryStream())
            {
                // make place for length prefix
                str.Seek(sizeof(int), SeekOrigin.Begin);

                // serialize message into stream
                await Task.Factory.StartNew(() => this.formatter.Serialize(str, message));

                // write length of message
                str.Seek(0, SeekOrigin.Begin);
                var lengthBytes = BitConverter.GetBytes((int)str.Length - sizeof(int));
                await str.WriteAsync(lengthBytes, 0, sizeof(int));
                str.Seek(0, SeekOrigin.Begin);

                await this.writeSemaphore.WaitAsync();
                try
                {
                    await str.CopyToAsync(this.outputStream);
                    await this.outputStream.FlushAsync();
                }
                finally
                {
                    this.writeSemaphore.Release();
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="MessageReceived" /> event.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="MessageBase" /> that has been received.
        /// </param>
        private void RaiseMessageReceived(MessageBase message) => this.MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));

        /// <summary>
        /// Raises the <see cref="ConnectionMessageReceived" /> event.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="ConnectionMessageBase" /> that has been received.
        /// </param>
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

                this.activationStateSemaphore.Wait();
                this.activationStateSemaphore.Dispose();

                this.writeSemaphore.Wait();
                this.writeSemaphore.Dispose();

                this.socket.Dispose();
                this.outputStream.Dispose();

                this.Cache.Dispose();
                this.connectionCache.Dispose();

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
                    var lengthBuffer = new RTBuffer(sizeof(int));
                    await this.inputStream.ReadAsync(lengthBuffer, sizeof(int), InputStreamOptions.Partial);

                    if (lengthBuffer.Length == sizeof(int))
                    {
                        // get the length of the message
                        var lengthBytes = lengthBuffer.ToArray();
                        var length = BitConverter.ToInt32(lengthBytes, 0);

                        // fill the buffer with actual message
                        var messageBuffer = new byte[length];
                        var currentLength = 0U;

                        while (currentLength < length)
                        {
                            var leftLength = (uint)(length - currentLength);
                            var tempBuffer = new RTBuffer(leftLength);
                            await this.inputStream.ReadAsync(tempBuffer, leftLength, InputStreamOptions.Partial);

                            tempBuffer.CopyTo(0, messageBuffer, (int)currentLength, (int)tempBuffer.Length);
                            currentLength += tempBuffer.Length;
                        }

                        using (var bufferStream = new MemoryStream(messageBuffer, false))
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
