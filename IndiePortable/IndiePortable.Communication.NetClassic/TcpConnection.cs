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
    using EncryptedConnection;

    /// <summary>
    /// Represents one end of a connection using the TCP protocol.
    /// </summary>
    /// <seealso cref="EncryptedConnection.ICryptableConnection{TAddress}" />
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


        private readonly TcpClient client;


        private readonly NetworkStream stream;


        private readonly SemaphoreSlim streamSemaphore = new SemaphoreSlim(1, 1);


        private readonly BinaryFormatter formatter = BinaryFormatter.CreateWithCoreSurrogates();


        private readonly ConnectionMessageDispatcher<IPPortAddressInfo> connectionCache;


        private readonly SemaphoreSlim activationStateSemaphore = new SemaphoreSlim(1, 1);


        private readonly AutoResetEvent keepAliveWaitHandle = new AutoResetEvent(false);


        private readonly CryptoManagerBase<PublicKeyInfo> cryptoManager;

        /// <summary>
        /// The backing field for the <see cref="KeepAliveFrequency" /> property.
        /// </summary>
        private readonly TimeSpan keepAliveFrequencyBacking;

        /// <summary>
        /// The backing field fort he <see cref="MaxKeepAliveTimeout" /> property.
        /// </summary>
        private readonly TimeSpan maxKeepAliveTimeoutBacking;

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

        /// <summary>
        /// The backing field for the <see cref="IsSessionEncrypted" /> property.
        /// </summary>
        private bool isSessionEncryptedBacking;

        /// <summary>
        /// The <see cref="StateTask" /> reading messages from the TCP stream.
        /// </summary>
        private StateTask messageReaderTask;

        /// <summary>
        /// The <see cref="StateTask" /> checking whether the communication partner is connected.
        /// </summary>
        private StateTask keepAliveCheckerTask;

        /// <summary>
        /// The <see cref="StateTask" /> sending keep-alive messages.
        /// </summary>
        private StateTask keepAliveSenderTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnection"/> class.
        /// </summary>
        /// <param name="client">
        ///     The <see cref="TcpClient" /> providing I/O operations for the <see cref="TcpConnection" />.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <param name="remoteAddress">
        ///     The address of the remote host.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if at least one of the following conditions applies:</para>
        ///     <list type="bullet">
        ///         <item><paramref name="client" /> is <c>null</c>.</item>
        ///         <item><paramref name="remoteAddress" /> is <c>null</c>.</item>
        ///     </list>
        /// </exception>
        public TcpConnection(TcpClient client, IPPortAddressInfo remoteAddress)
            : this(client, remoteAddress, new RsaCryptoManager())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnection" /> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="remoteAddress">The remote address.</param>
        /// <param name="keepAliveFrequency">The keep alive frequency.</param>
        /// <param name="maxKeepAliveTimeout">The maximum keep alive timeout.</param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if at least one of the following conditions applies:</para>
        ///     <list type="bullet">
        ///         <item><paramref name="client" /> is <c>null</c>.</item>
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
        public TcpConnection(TcpClient client, IPPortAddressInfo remoteAddress, TimeSpan keepAliveFrequency, TimeSpan maxKeepAliveTimeout)
            : this(client, remoteAddress, new RsaCryptoManager(), keepAliveFrequency, maxKeepAliveTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnection"/> class.
        /// </summary>
        /// <param name="client">
        ///     The <see cref="TcpClient" /> providing I/O operations for the <see cref="TcpConnection" />.
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
        ///         <item><paramref name="client" /> is <c>null</c>.</item>
        ///         <item><paramref name="remoteAddress" /> is <c>null</c>.</item>
        ///         <item><paramref name="cryptoManager" /> is <c>null</c>.</item>
        ///     </list>
        /// </exception>
        public TcpConnection(
            TcpClient client,
            IPPortAddressInfo remoteAddress,
            CryptoManagerBase<PublicKeyInfo> cryptoManager)
            : this(client, remoteAddress, cryptoManager, DefaultKeepAliveFrequency, DefaultMaxKeepAliveTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpConnection"/> class.
        /// </summary>
        /// <param name="client">
        ///     The <see cref="TcpClient" /> providing I/O operations for the <see cref="TcpConnection" />.
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
        ///         <item><paramref name="client" /> is <c>null</c>.</item>
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
            TcpClient client,
            IPPortAddressInfo remoteAddress,
            CryptoManagerBase<PublicKeyInfo> cryptoManager,
            TimeSpan keepAliveFrequency,
            TimeSpan maxKeepAliveTimeout)
        {
            if (object.ReferenceEquals(client, null))
            {
                throw new ArgumentNullException(nameof(client));
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
                maxKeepAliveTimeout >= TimeSpan.MaxValue ||
                maxKeepAliveTimeout.TotalMilliseconds > keepAliveFrequency.TotalMilliseconds * 4)
            {
                throw new ArgumentOutOfRangeException(nameof(maxKeepAliveTimeout));
            }

            this.keepAliveFrequencyBacking = keepAliveFrequency;
            this.maxKeepAliveTimeoutBacking = maxKeepAliveTimeout;

            this.cryptoManager = cryptoManager;

            this.client = client;
            this.remoteAddressBacking = remoteAddress;
            this.isConnectedBacking = true;

            this.cacheBacking = new MessageDispatcher(this);
            this.connectionCache = new ConnectionMessageDispatcher<IPPortAddressInfo>(this);

            this.handlerDisconnect = new ConnectionMessageHandler<ConnectionDisconnectRequest>(this.HandleDisconnect);
            this.handlerKeepAlive = new ConnectionMessageHandler<ConnectionMessageKeepAlive>(this.HandleKeepAlive);
            this.handlerContent = new ConnectionMessageHandler<ConnectionContentMessage>(this.HandleContent);
            this.handlerEncryptRequest = new ConnectionMessageHandler<ConnectionEncryptRequest>(this.HandleEncryptRequest);

            this.connectionCache.AddMessageHandler(this.handlerDisconnect);
            this.connectionCache.AddMessageHandler(this.handlerKeepAlive);
            this.connectionCache.AddMessageHandler(this.handlerContent);

            this.stream = this.client.GetStream();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TcpConnection"/> class.
        /// </summary>
        ~TcpConnection()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Raised when a <see cref="MessageBase" /> object has been received.
        /// </summary>
        /// <remarks>
        ///     <para>Implements <see cref="IMessageReceiver.MessageReceived" /> implicitly.</para>
        /// </remarks>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Raised when a <see cref="ConnectionMessageBase" /> has been received.
        /// </summary>
        /// <remarks>
        ///     <para>Implements <see cref="IConnection{TAddress}.ConnectionMessageReceived" /> implicitly.</para>
        /// </remarks>
        public event EventHandler<ConnectionMessageReceivedEventArgs> ConnectionMessageReceived;

        /// <summary>
        /// Raised when the <see cref="TcpConnection" /> has been disconnected.
        /// </summary>
        /// <remarks>
        ///     <para>Implements <see cref="IConnection{TAddress}.Disconnected" /> implicitly.</para>
        /// </remarks>
        public event EventHandler Disconnected;

        /// <summary>
        /// Gets the <see cref="MessageDispatcher" /> acting as a message cache.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="MessageDispatcher" /> acting as a message cache.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="IMessageReceiver.Cache" /> implicitly.</para>
        /// </remarks>
        public MessageDispatcher Cache => this.cacheBacking;

        /// <summary>
        /// Gets a value indicating whether the <see cref="TcpConnection" /> is activated.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="TcpConnection" /> is activated; otherwise, <c>false</c>.
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
        /// <remarks>
        ///     <para>Implements <see cref="ICryptableConnection{TAddress}.IsSessionEncrypted" /> implicitly.</para>
        /// </remarks>
        public bool IsSessionEncrypted => this.isSessionEncryptedBacking;

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
        /// Activates the <see cref="TcpConnection" />.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     <para>
        ///         Thrown if the <see cref="TcpConnection" /> has already been activated.
        ///         Check the <see cref="IsActivated" /> property.
        ///     </para>
        /// </exception>
        /// <remarks>
        ///     <para>Call this method to allow incoming and outgoing messages to be sent or received.</para>
        ///     <para>Implements <see cref="IConnection{TAddress}.Activate()" /> implicitly.</para>
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
        public void Disconnect() => this.Disconnect(TimeSpan.FromSeconds(10d));
        
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

            if (!this.IsConnected || this.isDisposed || !this.IsActivated)
            {
                throw new InvalidOperationException();
            }

            try
            {
                var rq = new ConnectionDisconnectRequest();
                this.SendConnectionMessage(rq);
                this.connectionCache.Wait<ConnectionDisconnectRequest, ConnectionDisconnectResponse>(rq, timeout);
            }
            finally
            {
                this.isConnectedBacking = false;
                this.messageReaderTask.Stop();
                this.keepAliveCheckerTask.Stop();
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
        /// Sends a <see cref="MessageBase" /> object through the <see cref="TcpConnection" />.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="MessageBase" /> that shall be sent.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="message" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <para>
        ///         Thrown if the <see cref="TcpConnection" /> has not been activated.
        ///         Check the <see cref="IsActivated" /> property. If it is <c>false</c>,
        ///         call the <see cref="Activate()" /> method before sending a message.
        ///     </para>
        /// </exception>
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
        /// <summary>
        /// Asynchronously sends a <see cref="MessageBase" /> object through the <see cref="TcpConnection" />.
        /// </summary>
        /// <param name="message">
        ///     The <see cref="MessageBase" /> that shall be sent.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <returns>
        /// The task processing the method.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="message" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <para>
        ///         Thrown if the <see cref="TcpConnection" /> has not been activated.
        ///         Check the <see cref="IsActivated" /> property. If it is <c>false</c>,
        ///         call the <see cref="Activate()" /> method before sending a message.
        ///     </para>
        /// </exception>
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

        /// <summary>
        /// Starts the encryption session for the <see cref="TcpConnection" />.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the session is already encrypted. Check the <see cref="IsSessionEncrypted" /> property.</para>
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
        ///     The task representing the method.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <para>Thrown if the session is already encrypted. Check the <see cref="IsSessionEncrypted" /> property.</para>
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

            using (var ms = new MemoryStream())
            {
                using (var encStream = new MemoryStream())
                {
                    this.formatter.Serialize(encStream, message);

                    var encryptedBytes = this.IsSessionEncrypted
                                       ? this.cryptoManager.Encrypt(encStream.ToArray())
                                       : encStream.ToArray();

                    var msgLengthBytes = BitConverter.GetBytes(encryptedBytes.Length);
                    ms.Write(msgLengthBytes, 0, sizeof(int));
                    ms.Write(encryptedBytes, 0, encryptedBytes.Length);
                }

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

            using (var ms = new MemoryStream())
            {
                using (var encStream = new MemoryStream())
                {
                    await Task.Factory.StartNew(() => this.formatter.Serialize(encStream, message));

                    var encryptedBytes = this.IsSessionEncrypted
                                       ? this.cryptoManager.Encrypt(encStream.ToArray())
                                       : encStream.ToArray();

                    var msgLengthBytes = BitConverter.GetBytes(encryptedBytes.Length);
                    await ms.WriteAsync(msgLengthBytes, 0, sizeof(int));
                    await ms.WriteAsync(encryptedBytes, 0, encryptedBytes.Length);
                }

                var toSend = ms.ToArray();
                await this.streamSemaphore.WaitAsync();
                try
                {
                    this.stream.Write(toSend, 0, toSend.Length);
                    this.stream.Flush();
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

                this.messageReaderTask?.Stop();
                this.keepAliveCheckerTask?.Stop();
                this.keepAliveSenderTask?.Stop();

                this.activationStateSemaphore.Dispose();

                this.streamSemaphore.Wait();
                this.stream.Dispose();
                this.streamSemaphore.Dispose();

                this.keepAliveWaitHandle.Close();

                this.cryptoManager?.Dispose();

                this.cacheBacking.Dispose();
                this.client.Client.Close();
                this.client.Close();
                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Reads messages from the TCP stream.
        /// </summary>
        /// <param name="taskConnection">
        ///     The <see cref="ITaskConnection" /> provinding communication between the caller and the callee thread.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="taskConnection" /> is <c>null</c>.</para>
        /// </exception>
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
            catch (Exception exc)
            {
                taskConnection.ThrowException(exc);
                return;
            }

            taskConnection.Return();
        }


        private async void KeepAliveChecker(ITaskConnection connection)
        {
            if (object.ReferenceEquals(connection, null))
            {
                throw new ArgumentNullException(nameof(connection));
            }

            try
            {
                while (!connection.MustFinish)
                {
                    if (!await Task.Factory.StartNew(() => this.keepAliveWaitHandle.WaitOne(this.MaxKeepAliveTimeout)))
                    {
                        try
                        {
                            this.Disconnect();
                        }
                        finally
                        {
                            connection.Stop();
                            this.Dispose();
                        }
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
