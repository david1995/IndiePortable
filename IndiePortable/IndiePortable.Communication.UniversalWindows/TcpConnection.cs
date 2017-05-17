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
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading;
    using System.Threading.Tasks;
    using AdvancedTasks;
    using Devices;
    using Devices.ConnectionMessages;
    using EncryptedConnection;
    using Formatter;
    using Messages;
    using Tcp;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;
    using RTBuffer = Windows.Storage.Streams.Buffer;

    /// <summary>
    /// Represents a connection between two TCP end points.
    /// </summary>
    /// <seealso cref="Devices.IConnection{TAddress}" />
    /// <seealso cref="IDisposable" />
    public sealed partial class TcpConnection
        : ICryptableConnection<IPPortAddressInfo>, IDisposable
    {
        /// <summary>
        /// Contains the default keep alive frequency of the <see cref="TcpConnection" /> of 5 seconds.
        /// </summary>
        public static readonly TimeSpan DefaultKeepAliveFrequency = TimeSpan.FromSeconds(5d);

        /// <summary>
        /// Contains the default maximum keep alive timeout of the <see cref="TcpConnection" /> of 10 seconds.
        /// </summary>
        public static readonly TimeSpan DefaultMaxKeepAliveTimeout = TimeSpan.FromSeconds(10d);

        /// <summary>
        /// The backing field for the <see cref="Cache" /> property.
        /// </summary>
        private readonly MessageDispatcher cacheBacking;

        /// <summary>
        /// The backing field for the <see cref="RemoteAddress" /> property.
        /// </summary>
        private readonly IPPortAddressInfo remoteAddressBacking;

        /// <summary>
        /// The <see cref="BinaryFormatter" /> used to serialize and deserialize messages.
        /// </summary>
        private readonly BinaryFormatter formatter = BinaryFormatter.CreateWithCoreSurrogates();


        private readonly ConnectionMessageDispatcher<IPPortAddressInfo> connectionCache;


        private readonly SemaphoreSlim writeSemaphore = new SemaphoreSlim(1, 1);


        private readonly SemaphoreSlim keepAliveSemaphore = new SemaphoreSlim(1, 1);


        private readonly CryptoManagerBase<PublicKeyInfo> cryptoManager;


        private readonly StreamSocket socket;


        private readonly IInputStream inputStream;

        /// <summary>
        /// The <see cref="Stream" /> serving as output.
        /// </summary>
        private readonly Stream outputStream;


        private readonly SemaphoreSlim activationStateSemaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// The backing field for the <see cref="KeepAliveFrequency" /> property.
        /// </summary>
        private readonly TimeSpan keepAliveFrequencyBacking;

        /// <summary>
        /// The backing field fort he <see cref="MaxKeepAliveTimeout" /> property.
        /// </summary>
        private readonly TimeSpan maxKeepAliveTimeoutBacking;

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

        /// <summary>
        /// The backing filed for the <see cref="IsSessionEncrypted" /> property.
        /// </summary>
        private bool isSessionEncryptedBacking;


        private StateTask messageReaderTask;

        
        private StateTask keepAliveCheckerTask;


        private StateTask keepAliveSenderTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnection"/> class.
        /// </summary>
        /// <param name="socket">
        ///     The <see cref="StreamSocket" /> providing I/O operations for the <see cref="TcpConnection" />.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <param name="remoteAddress">
        ///     The address of the remote host.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if:</para>
        ///     <list type="bullet">
        ///         <item>
        ///             <description><paramref name="socket" /> is <c>null</c>.</description>
        ///         </item>
        ///         <item>
        ///             <description><paramref name="remoteAddress" /> is <c>null</c>.</description>
        ///         </item>
        ///     </list>
        /// </exception>
        public TcpConnection(StreamSocket socket, IPPortAddressInfo remoteAddress)
            : this(socket, remoteAddress, new RsaCryptoManager())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnection" /> class.
        /// </summary>
        /// <param name="socket">The client.</param>
        /// <param name="remoteAddress">The remote address.</param>
        /// <param name="keepAliveFrequency">The keep alive frequency.</param>
        /// <param name="maxKeepAliveTimeout">The maximum keep alive timeout.</param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if at least one of the following conditions applies:</para>
        ///     <list type="bullet">
        ///         <item><paramref name="socket" /> is <c>null</c>.</item>
        ///         <item><paramref name="remoteAddress" /> is <c>null</c>.</item>
        ///     </list>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if at least one of the following conditions applies:</para>
        ///     <list type="bullet">
        ///         <item><paramref name="keepAliveFrequency" /> is <c>null</c>.</item>
        ///         <item><paramref name="maxKeepAliveTimeout" /> is <c>null</c>.</item>
        ///     </list>
        /// </exception>
        public TcpConnection(StreamSocket socket, IPPortAddressInfo remoteAddress, TimeSpan keepAliveFrequency, TimeSpan maxKeepAliveTimeout)
            : this(socket, remoteAddress, new RsaCryptoManager(), keepAliveFrequency, maxKeepAliveTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnection"/> class.
        /// </summary>
        /// <param name="socket">
        ///     The <see cref="StreamSocket" /> providing I/O operations for the <see cref="TcpConnection" />.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <param name="remoteAddress">
        ///     The address of the remote host.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <param name="cryptoManager">
        ///     The <see cref="CryptoManagerBase{T}" /> managing the encryption and decryption of messages.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if at least one of the following conditions applies:</para>
        ///     <list type="bullet">
        ///         <item><paramref name="socket" /> is <c>null</c>.</item>
        ///         <item><paramref name="remoteAddress" /> is <c>null</c>.</item>
        ///         <item><paramref name="cryptoManager" /> is <c>null</c>.</item>
        ///     </list>
        /// </exception>
        public TcpConnection(StreamSocket socket, IPPortAddressInfo remoteAddress, CryptoManagerBase<PublicKeyInfo> cryptoManager)
            : this(socket, remoteAddress, cryptoManager, DefaultKeepAliveFrequency, DefaultMaxKeepAliveTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnection"/> class.
        /// </summary>
        /// <param name="socket">
        ///     The <see cref="StreamSocket" /> providing I/O operations for the <see cref="TcpConnection" />.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <param name="remoteAddress">
        ///     The address of the remote host.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <param name="cryptoManager">
        ///     The <see cref="CryptoManagerBase{T}" /> managing the encryption and decryption of messages.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <param name="keepAliveFrequency">
        ///     The interval of sending keep-alive messages.
        ///     Must be greater than <see cref="TimeSpan.Zero" />.
        /// </param>
        /// <param name="maxKeepAliveTimeout">
        ///     The maximum acceptable interval between sending two keep-alive messages.
        ///     Must be greater than <paramref name="keepAliveFrequency" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if at least one of the following conditions applies:</para>
        ///     <list type="bullet">
        ///         <item><paramref name="socket" /> is <c>null</c>.</item>
        ///         <item><paramref name="remoteAddress" /> is <c>null</c>.</item>
        ///         <item><paramref name="cryptoManager" /> is <c>null</c>.</item>
        ///     </list>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if at least one of the following conditions applies:</para>
        ///     <list type="bullet">
        ///         <item><paramref name="keepAliveFrequency" /> is <c>null</c>.</item>
        ///         <item><paramref name="maxKeepAliveTimeout" /> is <c>null</c>.</item>
        ///     </list>
        /// </exception>
        public TcpConnection(
            StreamSocket socket,
            IPPortAddressInfo remoteAddress,
            CryptoManagerBase<PublicKeyInfo> cryptoManager,
            TimeSpan keepAliveFrequency,
            TimeSpan maxKeepAliveTimeout)
        {
            if (object.ReferenceEquals(socket, null))
            {
                throw new ArgumentNullException(nameof(socket));
            }

            if (object.ReferenceEquals(remoteAddress, null))
            {
                throw new ArgumentNullException(nameof(remoteAddress));
            }

            if (object.ReferenceEquals(cryptoManager, null))
            {
                throw new ArgumentNullException(nameof(cryptoManager));
            }

            if (keepAliveFrequency <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(keepAliveFrequency));
            }

            if (maxKeepAliveTimeout <= keepAliveFrequency ||
                maxKeepAliveTimeout >= TimeSpan.MaxValue)
            { 
                throw new ArgumentOutOfRangeException(nameof(maxKeepAliveTimeout));
            }

            this.socket = socket;
            this.isConnectedBacking = true;
            this.remoteAddressBacking = remoteAddress;
            this.inputStream = this.socket.InputStream;
            this.outputStream = this.socket.OutputStream.AsStreamForWrite();

            this.cacheBacking = new MessageDispatcher(this);
            this.cryptoManager = cryptoManager;

            this.keepAliveFrequencyBacking = keepAliveFrequency;
            this.maxKeepAliveTimeoutBacking = maxKeepAliveTimeout;

            this.connectionCache = new ConnectionMessageDispatcher<IPPortAddressInfo>(this);

            this.handlerDisconnect = new ConnectionMessageHandler<ConnectionDisconnectRequest>(this.HandleDisconnect);
            this.handlerKeepAlive = new ConnectionMessageHandler<ConnectionMessageKeepAlive>(this.HandleKeepAlive);
            this.handlerContent = new ConnectionMessageHandler<ConnectionContentMessage>(this.HandleContent);
            this.handlerEncryptionRequest = new ConnectionMessageHandler<ConnectionEncryptRequest>(this.HandleEncryptionRequest);

            this.connectionCache.AddMessageHandler(this.handlerDisconnect);
            this.connectionCache.AddMessageHandler(this.handlerKeepAlive);
            this.connectionCache.AddMessageHandler(this.handlerContent);
            this.connectionCache.AddMessageHandler(this.handlerEncryptionRequest);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TcpConnection" /> class.
        /// </summary>
        ~TcpConnection()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Raised when a <see cref="ConnectionMessageBase" /> has been received.
        /// </summary>
        /// <remarks>
        ///     <para>Implements <see cref="IConnection{TAddress}.ConnectionMessageReceived" /> implicitly.</para>
        /// </remarks>
        public event EventHandler<ConnectionMessageReceivedEventArgs> ConnectionMessageReceived;

        /// <summary>
        /// Raised when a <see cref="MessageBase" /> object has been received.
        /// </summary>
        /// <remarks>
        ///     <para>Implements <see cref="IMessageReceiver.MessageReceived" /> implicitly.</para>
        /// </remarks>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Raised when the <see cref="TcpConnection" /> has been disconnected.
        /// </summary>
        /// <remarks>
        ///     <para>Implements <see cref="IConnection{TAddress}.Disconnected" /> implicitly.</para>
        /// </remarks>
        public event EventHandler Disconnected;

        /// <summary>
        /// Gets the <see cref="MessageDispatcher" /> acting as a message cache for the <see cref="TcpConnection" />.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="MessageDispatcher" /> acting as a message cache for the <see cref="TcpConnection" />.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="IMessageReceiver.Cache" /> implicitly.</para>
        /// </remarks>
        public MessageDispatcher Cache => this.cacheBacking;

        /// <summary>
        /// Gets a value indicating whether the <see cref="TcpConnection" /> is activated.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="TcpConnection" /> is activated; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         Messages can only be sent or received if <see cref="IsActivated" /> is <c>true</c>.
        ///         Otherwise, an <see cref="InvalidOperationException" /> will be thrown.
        ///     </para>
        ///     <para>Implements <see cref="IConnection{TAddress}.IsActivated" /> implicitly.</para>
        /// </remarks>
        public bool IsActivated => this.isActivatedBacking;

        /// <summary>
        /// Gets a value indicating whether the <see cref="TcpConnection" /> is connected to the other end.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="TcpConnection" /> is connected; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="IConnection{TAddress}.IsConnected" /> implicitly.</para>
        /// </remarks>
        public bool IsConnected => this.isConnectedBacking;

        /// <summary>
        /// Gets a value indicating whether the current connection session is session encrypted.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the current connection session is encrypted; otherwise, <c>false</c>.
        /// </value>
        public bool IsSessionEncrypted => this.isSessionEncryptedBacking;

        /// <summary>
        /// Gets the interval of sending keep-alive messages.
        /// </summary>
        /// <value>
        ///     Contains the interval of sending keep-alive messages.
        /// </value>
        public TimeSpan KeepAliveFrequency => this.keepAliveFrequencyBacking;

        /// <summary>
        /// Gets the maximum acceptable interval between sending two keep-alive messages.
        /// </summary>
        /// <value>
        ///     Contains the maximum acceptable interval between sending two keep-alive messages.
        /// </value>
        public TimeSpan MaxKeepAliveTimeout => this.maxKeepAliveTimeoutBacking;

        /// <summary>
        /// Gets the remote address of the other connection end.
        /// </summary>
        /// <value>
        ///     Contains the remote address of the other connection end.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="IConnection{TAddress}.RemoteAddress" /> implicitly.</para>
        /// </remarks>
        public IPPortAddressInfo RemoteAddress => this.remoteAddressBacking;

        /// <summary>
        /// Activates the <see cref="TcpConnection" />.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the <see cref="TcpConnection" /> has already been activated.</para>
        /// </exception>
        /// <remarks>
        ///     <para>Implements <see cref="IConnection{TAddress}.Activate()" /> implicitly.</para>
        ///     <para>Call this method to allow incoming and outgoing messages to be sent or received.</para>
        /// </remarks>
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
                this.keepAliveCheckerTask = new StateTask(this.KeepAliveChecker);
                this.keepAliveSenderTask = new StateTask(this.KeepAliveSender);
                this.isActivatedBacking = true;
            }
            finally
            {
                this.activationStateSemaphore.Release();
            }
        }

        /// <summary>
        /// Disconnects the two end points.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if one of the following conditions is true:</para>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>The <see cref="IsActivated" /> property is <c>false</c>.</description>
        ///         </item>
        ///         <item>
        ///             <description>The <see cref="TcpConnection" /> is disposed.</description>
        ///         </item>
        ///         <item>
        ///             <description>The <see cref="IsConnected" /> property is <c>false</c>.</description>
        ///         </item>
        ///     </list>
        /// </exception>
        /// <remarks>
        ///     <para>Implements <see cref="IConnection{TAddress}.Disconnect()" /> implicitly.</para>
        /// </remarks>
        public void Disconnect() => this.Disconnect(TimeSpan.FromSeconds(5d));

        /// <summary>
        /// Disconnects the two end points.
        /// </summary>
        /// <param name="timeout">
        ///     The duration that shall be waited until a disconnect response has been received.
        ///     Must be greater than <see cref="TimeSpan.Zero" />.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if <paramref name="timeout" /> is smaller or equals <see cref="TimeSpan.Zero" />.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if one of the following conditions is true:</para>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>The <see cref="IsActivated" /> property is <c>false</c>.</description>
        ///         </item>
        ///         <item>
        ///             <description>The <see cref="TcpConnection" /> is disposed.</description>
        ///         </item>
        ///         <item>
        ///             <description>The <see cref="IsConnected" /> property is <c>false</c>.</description>
        ///         </item>
        ///     </list>
        /// </exception>
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

            try
            {
                this.keepAliveCheckerTask.Stop();
                var rq = new ConnectionDisconnectRequest();
                this.SendConnectionMessage(rq);
                this.connectionCache.Wait<ConnectionDisconnectRequest, ConnectionDisconnectResponse>(rq, timeout);
            }
            catch (IOException)
            {
            }
            finally
            {
                this.messageReaderTask.Stop();
                this.isConnectedBacking = false;
                this.RaiseDisconnected();
                this.Dispose();
            }
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

        /// <summary>
        /// Starts the encryption session for the <see cref="TcpConnection" />.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the encryption session has already been started. Check the <see cref="IsSessionEncrypted" /> property.</para>
        /// </exception>
        public void StartEncryptionSession()
        {
            if (this.IsSessionEncrypted)
            {
                throw new InvalidOperationException();
            }

            var rq = new ConnectionEncryptRequest(this.cryptoManager.LocalPublicKey);
            this.SendConnectionMessage(rq);
            var rsp = this.connectionCache.Wait<ConnectionEncryptRequest, ConnectionEncryptResponse>(rq, TimeSpan.FromSeconds(5));
            if (object.ReferenceEquals(rsp, null))
            {
                throw new InvalidOperationException();
            }

            this.cryptoManager.StartSession(rsp.PublicKey);
            this.isSessionEncryptedBacking = true;
        }

        /// <summary>
        /// Asynchronously starts the encryption session for the <see cref="TcpConnection" />.
        /// </summary>
        /// <returns>
        ///     The task executing the method.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the encryption session has already been started. Check the <see cref="IsSessionEncrypted" /> property.</para>
        /// </exception>
        public async Task StartEncryptionSessionAsync()
        {
            if (this.IsSessionEncrypted)
            {
                throw new InvalidOperationException();
            }

            var rq = new ConnectionEncryptRequest(this.cryptoManager.LocalPublicKey);
            await this.SendConnectionMessageAsync(rq);
            var rsp = await this.connectionCache.WaitAsync<ConnectionEncryptRequest, ConnectionEncryptResponse>(rq, TimeSpan.FromSeconds(5));
            if (object.ReferenceEquals(rsp, null))
            {
                throw new InvalidOperationException();
            }

            this.cryptoManager.StartSession(rsp.PublicKey);
            this.isSessionEncryptedBacking = true;
        }

        /// <summary>
        /// Sends a <see cref="MessageBase" /> object to the other connection end.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="MessageBase" /> that shall be sent.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="message" /> is <c>null</c>.</para>
        /// </exception>
        public void SendMessage(MessageBase message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.SendConnectionMessage(new ConnectionContentMessage(message));
        }

        /// <summary>
        /// Sends a <see cref="MessageBase" /> object asynchronously to the other connection end.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="MessageBase" /> that shall be sent.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <returns>
        ///     The task processing the method.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="message" /> is <c>null</c>.</para>
        /// </exception>
        public async Task SendMessageAsync(MessageBase message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            await this.SendConnectionMessageAsync(new ConnectionContentMessage(message));
        }
        
        /// <summary>
        /// Sends a connection message.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="ConnectionMessageBase" /> that shall be sent.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the <see cref="TcpConnection" /> is not activated. Check the <see cref="IsActivated" /> property.</para>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="message" /> is <c>null</c>.</para>
        /// </exception>
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
                using (var tempstr = new MemoryStream())
                {
                    // serialize message into stream
                    this.formatter.Serialize(tempstr, message);
                    
                    byte[] toWrite;
                    if (this.IsSessionEncrypted)
                    {
                        var toEncrypt = tempstr.ToArray();
                        toWrite = this.cryptoManager.Encrypt(toEncrypt);
                    }
                    else
                    {
                        toWrite = tempstr.ToArray();
                    }

                    var lengthBytes = BitConverter.GetBytes(toWrite.Length);
                    str.Write(lengthBytes, 0, sizeof(int));

                    str.Write(toWrite, 0, toWrite.Length);
                }

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

        /// <summary>
        /// Sends a connection message asynchonously.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="ConnectionMessageBase" /> that shall be sent.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <returns>
        ///     The running task.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the <see cref="TcpConnection" /> is not activated. Check the <see cref="IsActivated" /> property.</para>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="message" /> is <c>null</c>.</para>
        /// </exception>
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
                using (var tempstr = new MemoryStream())
                {
                    // serialize message into stream
                    await Task.Factory.StartNew(() => this.formatter.Serialize(tempstr, message));

                    byte[] toWrite;
                    if (this.IsSessionEncrypted)
                    {
                        var toEncrypt = tempstr.ToArray();
                        toWrite = this.cryptoManager.Encrypt(toEncrypt);
                    }
                    else
                    {
                        toWrite = tempstr.ToArray();
                    }

                    var lengthBytes = BitConverter.GetBytes(toWrite.Length);
                    await str.WriteAsync(lengthBytes, 0, sizeof(int));

                    await str.WriteAsync(toWrite, 0, toWrite.Length);
                }

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
        /// Raises the <see cref="Disconnected" /> event.
        /// </summary>
        private void RaiseDisconnected() => this.Disconnected?.Invoke(this, EventArgs.Empty);

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

                this.keepAliveCheckerTask?.Stop();
                this.keepAliveSenderTask?.Stop();
                this.messageReaderTask?.Stop();

                this.keepAliveSemaphore.Wait();
                this.keepAliveSemaphore.Dispose();

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
                            if (this.formatter.TryDeserialize(bufferStream, out ConnectionMessageBase message))
                            {
                                new Task(() => this.RaiseConnectionMessageReceived(message)).Start();
                            }
                            else
                            {
                                // if deserialization fails, "reset" connection by consuming all bytes in the socket buffer
                                RTBuffer clearBuffer;
                                do
                                {
                                    clearBuffer = new RTBuffer(sizeof(long));
                                    await this.inputStream.ReadAsync(lengthBuffer, sizeof(long), InputStreamOptions.Partial);
                                }
                                while (clearBuffer.Length == sizeof(long));
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


        private void KeepAliveChecker(ITaskConnection connection)
        {
            if (object.ReferenceEquals(connection, null))
            {
                throw new ArgumentNullException(nameof(connection));
            }

            try
            {
                while (!connection.MustFinish)
                {
                    if (!this.keepAliveSemaphore.Wait(TimeSpan.FromSeconds(10d)))
                    {
                        this.Disconnect();
                        connection.Stop();
                    }
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception exc)
            {
                connection.ThrowException(exc);
                return;
            }

            connection.Return();
        }


        private async void KeepAliveSender(ITaskConnection connection)
        {
            if (object.ReferenceEquals(connection, null))
            {
                throw new ArgumentNullException(nameof(connection));
            }

            try
            {
                while (!connection.MustFinish)
                {
                    await this.SendConnectionMessageAsync(new ConnectionMessageKeepAlive());

                    await Task.Delay(this.KeepAliveFrequency);
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception exc)
            {
                connection.ThrowException(exc);
                return;
            }

            connection.Return();
        }
    }
}
