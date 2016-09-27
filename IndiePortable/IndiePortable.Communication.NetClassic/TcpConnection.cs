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
    using Formatter.MscorlibSurrogates;
    using AdvancedTasks;
    using Messages;
    using System.IO;
    using Tcp;
    using System.Threading;


    public sealed class TcpConnection
        : IConnection<IPAddressInfo>, IDisposable
    {
        /// <summary>
        /// The backing field for the <see cref="Cache" /> property.
        /// </summary>
        private readonly MessageDispatcher cacheBacking;

        /// <summary>
        /// The backing field for the <see cref="RemoteAddress" /> property.
        /// </summary>
        private readonly IPAddressInfo remoteAddressBacking;


        private readonly TcpClient client;


        private readonly NetworkStream stream;


        private readonly SemaphoreSlim streamSemaphore = new SemaphoreSlim(1, 1);


        private readonly BinaryFormatter formatter = new BinaryFormatter(new[] { new MscorlibSurrogateSelector() });

        /// <summary>
        /// The backing field for the <see cref="IsDisposed" /> property.
        /// </summary>
        private bool isDisposedBacking;

        /// <summary>
        /// The backing field for the <see cref="IsConnected" /> property.
        /// </summary>
        private bool isConnectedBacking;

        /// <summary>
        /// The backing field for the <see cref="IsActivated" /> property.
        /// </summary>
        private bool isActivatedBacking;


        private StateTask messageReaderTask;


        public TcpConnection(TcpClient client, IPAddressInfo remoteAddress)
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

            this.client = client;
            this.remoteAddressBacking = remoteAddress;
            this.isConnectedBacking = true;

            this.stream = this.client.GetStream();

            this.TcpMessageRecieved += this.TcpContentMessageRecieved;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TcpConnection"/> class.
        /// </summary>
        ~TcpConnection()
        {
            this.Dispose(false);
        }


        public event EventHandler<MessageReceivedEventArgs> MessageReceived;


        private event EventHandler<TcpMessageReceivedEventArgs> TcpMessageRecieved;


        public bool IsDisposed => this.isDisposedBacking;


        public MessageDispatcher Cache => this.cacheBacking;


        public bool IsConnected => this.isConnectedBacking;


        public bool IsActivated => this.isActivatedBacking;


        public IPAddressInfo RemoteAddress => this.remoteAddressBacking;


        public void Activate()
        {
            this.messageReaderTask = new StateTask(this.MessageReader);
            this.isActivatedBacking = true;
        }


        public void Disconnect()
        {
            this.SendConnectionMessage();

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
                this.SendConnectionMessage(new TcpContentMessage(message));
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
                await this.SendConnectionMessageAsync(new TcpContentMessage(message));
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException)
            {
            }
        }


        private void SendConnectionMessage(TcpConnectionMessage message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            using (var ms = new MemoryStream())
            {
                this.formatter.Serialize(ms, message);

                var bytesToSend = new byte[ms.Length + sizeof(int)];

                using (var sendMemstr = new MemoryStream(bytesToSend))
                {
                    sendMemstr.Write(BitConverter.GetBytes((int)ms.Length), 0, sizeof(int));
                    ms.CopyTo(sendMemstr);

                    this.streamSemaphore.Wait();
                    try
                    {
                        sendMemstr.WriteTo(this.stream);
                    }
                    finally
                    {
                        this.streamSemaphore.Release();
                    }
                }
            }
        }


        private async Task SendConnectionMessageAsync(TcpConnectionMessage msg)
        {
            if (object.ReferenceEquals(msg, null))
            {
                throw new ArgumentNullException(nameof(msg));
            }

            using (var ms = new MemoryStream())
            {
                await Task.Factory.StartNew(() => this.formatter.Serialize(ms, msg));

                var bytesToSend = new byte[ms.Length + sizeof(int)];
                ms.Seek(0, SeekOrigin.Begin);

                using (var sendMemstr = new MemoryStream(bytesToSend))
                {
                    await sendMemstr.WriteAsync(BitConverter.GetBytes((int)ms.Length), 0, sizeof(int));
                    await ms.CopyToAsync(sendMemstr);

                    await this.streamSemaphore.WaitAsync();
                    try
                    {
                        sendMemstr.WriteTo(this.stream);
                    }
                    finally
                    {
                        this.streamSemaphore.Release();
                    }
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
        /// Raises the <see cref="TcpMessageRecieved" /> event.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="TcpConnectionMessage" /> that has been received.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="message" /> is <c>null</c>.</para>
        /// </exception>
        private void RaiseTcpMessageRecieved(TcpConnectionMessage message)
            => this.TcpMessageRecieved?.Invoke(this, new TcpMessageReceivedEventArgs(message));

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (disposing)
                {
                }

                this.messageReaderTask?.Stop();

                this.streamSemaphore.Wait();
                this.stream.Dispose();
                this.streamSemaphore.Dispose();

                this.cacheBacking.Dispose();
                this.client.Client.Close();
                this.client.Close();
                this.isDisposedBacking = true;
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
                            TcpConnectionMessage message;
                            if (this.formatter.TryDeserialize(bufferStream, out message))
                            {
                                this.RaiseTcpMessageRecieved(message);
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

        private void TcpContentMessageRecieved(object sender, TcpMessageReceivedEventArgs e)
        {
            var msg = e.Message as TcpContentMessage;
            if (!object.ReferenceEquals(msg, null))
            {
                this.RaiseMessageReceived(msg.Content);
            }
        }
    }
}
