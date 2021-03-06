﻿// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionMessageDispatcher.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionMessageDispatcher class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using Collections.Linq;
    using ConnectionMessages;

    /// <summary>
    /// Manages received <see cref="ConnectionMessageBase" /> instances.
    /// </summary>
    /// <typeparam name="TAddress">
    ///     The type of the addresses.
    /// </typeparam>
    /// <seealso cref="IDisposable" />
    public sealed class ConnectionMessageDispatcher<TAddress>
        : IDisposable
        where TAddress : IAddressInfo
    {

        private readonly IConnection<TAddress> connection;


        private readonly DynamicArray<KeyValuePair<Type, IConnectionMessageHandler>> messageHandlers =
            new DynamicArray<KeyValuePair<Type, IConnectionMessageHandler>>();

        /// <summary>
        /// The list containing all currently waiting message response tasks.
        /// </summary>
        private readonly DynamicArray<IWaitingTask> waitingTasks = new DynamicArray<IWaitingTask>();

        /// <summary>
        /// Indicates whether the <see cref="ConnectionMessageDispatcher{TAddress}" /> is disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMessageDispatcher{TAddress}" /> class.
        /// </summary>
        /// <param name="connection">
        ///     The <see cref="IConnection{TAddress}" /> receiving the messages.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="connection" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if <paramref name="connection" /> is either activated or not connected.</para>
        /// </exception>
        public ConnectionMessageDispatcher(IConnection<TAddress> connection)
        {
            if (object.ReferenceEquals(connection, null))
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (!connection.IsConnected || connection.IsActivated)
            {
                throw new ArgumentException();
            }

            this.connection = connection;
            this.connection.ConnectionMessageReceived += this.Connection_MessageReceived;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ConnectionMessageDispatcher{TAddress}" /> class.
        /// </summary>
        ~ConnectionMessageDispatcher()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Adds a message handler to the <see cref="ConnectionMessageDispatcher{TAddress}" />.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the messages that shall be handled.
        ///     Must derive from <see cref="ConnectionMessageBase" />.
        /// </typeparam>
        /// <param name="messageHandler">
        ///     The <see cref="ConnectionMessageHandler{T}" /> that shall be added.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="messageHandler" /> is <c>null</c>.</para>
        /// </exception>
        public void AddMessageHandler<T>(ConnectionMessageHandler<T> messageHandler)
            where T : ConnectionMessageBase
        {
            if (object.ReferenceEquals(messageHandler, null))
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }

            this.messageHandlers.Add(new KeyValuePair<Type, IConnectionMessageHandler>(typeof(T), messageHandler));
        }

        /// <summary>
        /// Removes the message handler specified by the message type that is handled by it.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the messages that are handled by the message handler to remove.
        ///     Must derive from <see cref="ConnectionMessageBase" />.
        /// </typeparam>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if no message handler handling a message of type <typeparamref name="T" /> is registered.</para>
        /// </exception>
        public void RemoveMessageHandler<T>()
            where T : ConnectionMessageBase
        {
            var toRemove = this.messageHandlers.FirstOrDefault(f => f.Key == typeof(T));
            if (object.ReferenceEquals(toRemove, null))
            {
                throw new ArgumentException("No message handler is registered for the specified message type.", nameof(T));
            }

            this.messageHandlers.Remove(toRemove);
        }

        /// <summary>
        /// Removes the message handler specified by the message type that is handled by it.
        /// </summary>
        /// <param name="t">
        ///     The type of the messages that are handled by the message handler to remove.
        ///     Must derive from <see cref="ConnectionMessageBase" />.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="t" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if no message handler handling a message of type <paramref name="t" /> is registered.</para>
        /// </exception>
        public void RemoveMessageHandler(Type t)
        {
            if (object.ReferenceEquals(t, null))
            {
                throw new ArgumentNullException(nameof(t));
            }
            
            var toRemove = this.messageHandlers.FirstOrDefault(f => f.Key == t);
            if (object.ReferenceEquals(toRemove, null))
            {
                throw new ArgumentException("No message handler is registered for the specified message type.", nameof(t));
            }

            this.messageHandlers.Remove(toRemove);
        }

        /// <summary>
        /// Waits for the response message of a specified request message.
        /// </summary>
        /// <typeparam name="TRequest">
        ///     The type of the request message.
        ///     Must derive from <see cref="ConnectionMessageRequestBase" />.
        /// </typeparam>
        /// <typeparam name="TResponse">
        ///     The type of the response message.
        ///     Must derive from <see cref="ConnectionMessageResponseBase{T}" />.
        /// </typeparam>
        /// <param name="request">
        ///     The request message of which the response shall be awaited.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <returns>
        ///     Returns the response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="request" /> is <c>null</c>.</para>
        /// </exception>
        public TResponse Wait<TRequest, TResponse>(TRequest request)
            where TRequest : ConnectionMessageRequestBase
            where TResponse : ConnectionMessageResponseBase<TRequest>
        {

            if (object.ReferenceEquals(request, null))
            {
                throw new ArgumentNullException(nameof(request));
            }
            
            using (var waitingTask = new WaitingTask<TRequest, TResponse>(request))
            {
                this.waitingTasks.Add(waitingTask);

                var msg = waitingTask.Wait();
                
                this.waitingTasks.Remove(waitingTask);

                return msg;
            }
        }

        /// <summary>
        /// Waits for the response message of a specified request message.
        /// </summary>
        /// <typeparam name="TRequest">
        ///     The type of the request message.
        ///     Must derive from <see cref="ConnectionMessageRequestBase" />.
        /// </typeparam>
        /// <typeparam name="TResponse">
        ///     The type of the response message.
        ///     Must derive from <see cref="ConnectionMessageResponseBase{T}" />.
        /// </typeparam>
        /// <param name="request">
        ///     The request message of which the response shall be awaited.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <param name="timeout">
        ///     The maximum time limit that shall be waited.
        ///     Must be greater than <see cref="TimeSpan.Zero" />.
        /// </param>
        /// <returns>
        ///     Returns the response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="request" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if <paramref name="timeout" /> is smaller or equals <see cref="TimeSpan.Zero" />.</para>
        /// </exception>
        /// <exception cref="TimeoutException">
        ///     <para>
        ///         Thrown if the <see cref="ConnectionMessageDispatcher{TAddress}" /> has been waiting
        ///         longer than <paramref name="timeout" /> for the response.
        ///     </para>
        /// </exception>
        public TResponse Wait<TRequest, TResponse>(TRequest request, TimeSpan timeout)
            where TRequest : ConnectionMessageRequestBase
            where TResponse : ConnectionMessageResponseBase<TRequest>
        {

            if (object.ReferenceEquals(request, null))
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }

            using (var waitingTask = new WaitingTask<TRequest, TResponse>(request))
            {
                this.waitingTasks.Add(waitingTask);

                var msg = waitingTask.Wait(timeout);

                this.waitingTasks.Remove(waitingTask);

                return msg;
            }
        }

        /// <summary>
        /// Asynchronously waits for the response message of a specified request message.
        /// </summary>
        /// <typeparam name="TRequest">
        ///     The type of the request message.
        ///     Must derive from <see cref="ConnectionMessageRequestBase" />.
        /// </typeparam>
        /// <typeparam name="TResponse">
        ///     The type of the response message.
        ///     Must derive from <see cref="ConnectionMessageResponseBase{T}" />.
        /// </typeparam>
        /// <param name="request">
        ///     The request message of which the response shall be awaited.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <returns>
        ///     The task waiting for the response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="request" /> is <c>null</c>.</para>
        /// </exception>
        public async Task<TResponse> WaitAsync<TRequest, TResponse>(TRequest request)
            where TRequest : ConnectionMessageRequestBase
            where TResponse : ConnectionMessageResponseBase<TRequest>
        {
            if (object.ReferenceEquals(request, null))
            {
                throw new ArgumentNullException(nameof(request));
            }
            
            using (var waitingTask = new WaitingTask<TRequest, TResponse>(request))
            {
                this.waitingTasks.Add(waitingTask);

                var msg = await waitingTask.WaitAsync();
                
                this.waitingTasks.Remove(waitingTask);

                return msg;
            }
        }

        /// <summary>
        /// Asynchronously waits for the response message of a specified request message with a specified time limit.
        /// </summary>
        /// <typeparam name="TRequest">
        ///     The type of the request message.
        ///     Must derive from <see cref="ConnectionMessageRequestBase" />.
        /// </typeparam>
        /// <typeparam name="TResponse">
        ///     The type of the response message.
        ///     Must derive from <see cref="ConnectionMessageResponseBase{T}" />.
        /// </typeparam>
        /// <param name="request">
        ///     The request message of which the response shall be awaited.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <param name="timeout">
        ///     The maximum time limit that shall be waited.
        ///     Must be greater than <see cref="TimeSpan.Zero" />.
        /// </param>
        /// <returns>
        ///     The task waiting for the response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="request" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para>Thrown if <paramref name="timeout" /> is smaller or equals <see cref="TimeSpan.Zero" />.</para>
        /// </exception>
        /// <exception cref="TimeoutException">
        ///     <para>
        ///         Thrown if the <see cref="ConnectionMessageDispatcher{TAddress}" /> has been waiting
        ///         longer than <paramref name="timeout" /> for the response.
        ///     </para>
        /// </exception>
        public async Task<TResponse> WaitAsync<TRequest, TResponse>(TRequest request, TimeSpan timeout)
            where TRequest : ConnectionMessageRequestBase
            where TResponse : ConnectionMessageResponseBase<TRequest>
        {
            if (object.ReferenceEquals(request, null))
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }

            using (var waitingTask = new WaitingTask<TRequest, TResponse>(request))
            {
                this.waitingTasks.Add(waitingTask);

                var msg = await waitingTask.WaitAsync(timeout);

                this.waitingTasks.Remove(waitingTask);

                return msg;
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
                
                this.waitingTasks.ForEach(t => t.Dispose());
                this.isDisposed = true;
            }
        }


        private async void Connection_MessageReceived(object sender, ConnectionMessageReceivedEventArgs e)
        {
            if (object.ReferenceEquals(e, null))
            {
                throw new ArgumentNullException(nameof(e));
            }

            await Task.Factory.StartNew(
                () =>
                {
                    this.messageHandlers.Where(h => h.Key.GetTypeInfo().IsAssignableFrom(e.Message.GetType().GetTypeInfo()))
                                        .ForEach(h => h.Value.HandleMessage(e.Message));

                    this.waitingTasks.ForEach(t => t.PushMessage(e.Message));
                });
        }


        private interface IWaitingTask
            : IDisposable
        {
            
            Type ResponseType { get; }

            bool PushMessage(ConnectionMessageBase message);
        }

        /// <summary>
        /// Describes a wrapper around a task waiting for a response message.
        /// </summary>
        /// <seealso cref="IDisposable" />
        private sealed class WaitingTask<TRequest, TResponse>
            : IDisposable, IWaitingTask
            where TRequest : ConnectionMessageRequestBase
            where TResponse : ConnectionMessageResponseBase<TRequest>
        {
            /// <summary>
            /// The backing field for the <see cref="RequestMessage" /> property.
            /// </summary>
            private readonly TRequest requestMessageBacking;

            /// <summary>
            /// The <see cref="AutoResetEvent" /> wait handle blocking the <see cref="Wait()" />
            /// and the <see cref="WaitAsync()" /> method.
            /// </summary>
            private readonly AutoResetEvent waitHandle;

            /// <summary>
            /// The <see cref="Messages.IMessageResponse" /> that has been received,
            /// or <c>null</c> if no response has been received yet.
            /// </summary>
            private TResponse response;

            /// <summary>
            /// Initializes a new instance of the <see cref="WaitingTask{TRequest, TResponse}" /> class.
            /// </summary>
            /// <param name="request">
            ///     The request <typeparamref name="TRequest" />.
            ///     Must not be <c>null</c>.
            /// </param>
            /// <exception cref="ArgumentNullException">
            ///     <para>Thrown if:</para>
            ///     <para>  - <paramref name="request" /> is <c>null</c>.</para>
            /// </exception>
            internal WaitingTask(TRequest request)
            {
                if (object.ReferenceEquals(request, null))
                {
                    throw new ArgumentNullException(nameof(request));
                }

                this.requestMessageBacking = request;
                this.waitHandle = new AutoResetEvent(false);
            }


            /// <summary>
            /// Finalizes an instance of the <see cref="WaitingTask{TRequest, TResponse}" /> class.
            /// </summary>
            ~WaitingTask()
            {
                this.Dispose(false);
            }


            internal TRequest RequestMessage => this.requestMessageBacking;

            /// <summary>
            /// Gets or sets a value indicating whether the <see cref="WaitingTask{TRequest, TResponse}" />is disposed.
            /// </summary>
            /// <value>
            ///     <c>true</c> if the <see cref="WaitingTask{TRequest, TResponse}" /> is disposed; otherwise, <c>false</c>.
            /// </value>
            internal bool IsDisposed { get; private set; }


            public Type ResponseType => typeof(TResponse);

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


            internal TResponse Wait()
            {
                this.waitHandle.WaitOne();
                return this.response;
            }


            internal TResponse Wait(TimeSpan timeout)
            {
                if (!this.waitHandle.WaitOne(timeout))
                {
                    throw new TimeoutException();
                }

                return this.response;
            }


            internal async Task<TResponse> WaitAsync()
            {
                await Task.Run(() => this.waitHandle.WaitOne());
                return this.response;
            }

            /// <summary>
            /// Asynchronously waits for the specified response message.
            /// </summary>
            /// <param name="timeout">
            ///     The maximum time limit that shall be waited.
            ///     Must be greater than <see cref="TimeSpan.Zero" />.
            /// </param>
            /// <returns>
            ///     The task waiting for the response message.
            /// </returns>
            /// <exception cref="ArgumentOutOfRangeException">
            ///     <para>Thrown if <paramref name="timeout" /> is smaller or equals <see cref="TimeSpan.Zero" />.</para>
            /// </exception>
            /// <exception cref="TimeoutException">
            ///     <para>
            ///         Thrown if the <see cref="WaitingTask{TRequest, TResponse}" /> has been waiting
            ///         longer than <paramref name="timeout" /> for a response.
            ///     </para>
            /// </exception>
            internal async Task<TResponse> WaitAsync(TimeSpan timeout)
            {
                if (timeout <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(timeout));
                }

                if (!await Task.Run(() => this.waitHandle.WaitOne(timeout)))
                {
                    throw new TimeoutException();
                }

                return this.response;
            }


            public bool PushMessage(ConnectionMessageBase response)
            {
                if (response == null)
                {
                    throw new ArgumentNullException(nameof(response));
                }

                var rsp = response as TResponse;
                if (object.ReferenceEquals(rsp, null))
                {
                    throw new ArgumentException();
                }

                var msgType = response.GetType().GetTypeInfo();

                if (rsp.RequestIdentifier == this.RequestMessage.MessageIdentifier)
                {
                    this.response = rsp;
                    this.waitHandle.Set();
                    return true;
                }

                return false;
            }

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

                    this.waitHandle.Dispose();

                    this.IsDisposed = true;
                }
            }
        }
    }
}
