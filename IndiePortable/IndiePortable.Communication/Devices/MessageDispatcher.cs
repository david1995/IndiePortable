// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageDispatcher.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the MessageDispatcher class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections.Linq;
    using Messages;

    /// <summary>
    /// Provides simplified methods for handling incoming messages and message response waiting.
    /// </summary>
    /// <seealso cref="IDisposable" />
    public class MessageDispatcher
        : IDisposable
    {
        /// <summary>
        /// The <see cref="SemaphoreSlim" /> controlling the thread access to <see cref="waitingTasks" />.
        /// </summary>
        private readonly SemaphoreSlim waitingTasksSemaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// The list containing all currently waiting message response tasks.
        /// </summary>
        private readonly List<WaitingTask> waitingTasks = new List<WaitingTask>();

        /// <summary>
        /// The source list of the <see cref="MessageHandlers" /> property.
        /// </summary>
        private readonly List<IMessageHandler> messageHandlers;

        /// <summary>
        /// The backing field for the <see cref="MessageHandlers" /> property.
        /// </summary>
        private readonly IReadOnlyCollection<IMessageHandler> messageHandlersBacking;


        private readonly SemaphoreSlim messageHandlersSemaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// The <see cref="IMessageReceiver" /> providing the messages.
        /// </summary>
        private readonly IMessageReceiver receiver;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDispatcher" /> class.
        /// </summary>
        /// <param name="receiver">
        ///     <para>The <see cref="IMessageReceiver" /> providing the messages.</para>
        /// </param>
        public MessageDispatcher(IMessageReceiver receiver)
        {
            if (object.ReferenceEquals(receiver, null))
            {
                throw new ArgumentNullException(nameof(receiver));
            }

            this.messageHandlers = new List<IMessageHandler>();
            this.messageHandlersBacking = new ReadOnlyCollection<IMessageHandler>(this.messageHandlers);

            this.receiver = receiver;
            this.receiver.MessageReceived += this.Receiver_ReceivedMessage;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MessageDispatcher" /> class.
        /// </summary>
        ~MessageDispatcher()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the read-only list of registered message handlers.
        /// </summary>
        /// <value>
        ///     Contains the read-only list of registered message handlers.
        /// </value>
        public IReadOnlyCollection<IMessageHandler> MessageHandlers => this.messageHandlersBacking;

        /// <summary>
        /// Gets a value indicating whether the <see cref="MessageDispatcher" /> is disposed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="MessageDispatcher" /> is disposed; otherwise <c>false</c>.
        /// </value>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Registers a message handler at the <see cref="MessageDispatcher" />.
        /// </summary>
        /// <param name="messageHandler">
        ///     The <see cref="IMessageHandler" /> that shall be registered.
        ///     Must not be <c>null</c>-
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="messageHandler" /> is <c>null</c>.</para>
        /// </exception>
        public void AddMessageHandler(IMessageHandler messageHandler)
        {
            if (object.ReferenceEquals(messageHandler, null))
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }

            this.messageHandlersSemaphore.Wait();
            try
            {
                this.messageHandlers.Add(messageHandler);
            }
            finally
            {
                this.messageHandlersSemaphore.Release();
            }
        }

        /// <summary>
        /// Unregisters a message handler at the <see cref="MessageDispatcher" />.
        /// </summary>
        /// <param name="messageHandler">
        ///     The <see cref="IMessageHandler" /> that shall be unregistered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="messageHandler" /> is <c>null</c>-</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="messageHandler" /> is not registered at the <see cref="MessageDispatcher" />.
        /// </exception>
        public void RemoveMessageHandler(IMessageHandler messageHandler)
        {
            if (object.ReferenceEquals(messageHandler, null))
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }

            this.messageHandlersSemaphore.Wait();
            try
            {
                if (this.messageHandlers.Remove(messageHandler))
                {
                    throw new ArgumentException(
                        "The specified message handler is not registered at the MessageDispatcher.",
                        nameof(messageHandler));
                }
            }
            finally
            {
                this.messageHandlersSemaphore.Release();
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


        public TResponse Wait<TRequest, TResponse>(TRequest request)
            where TRequest : MessageRequestBase
            where TResponse : MessageResponseBase<TRequest>, IMessageResponse
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            this.waitingTasksSemaphore.Wait();
            using (var waitingTask = new WaitingTask(request, typeof(TResponse)))
            {
                this.waitingTasks.Add(waitingTask);
                this.waitingTasksSemaphore.Release();

                var msg = waitingTask.Wait<TRequest, TResponse>();

                this.waitingTasksSemaphore.Wait();
                this.waitingTasks.Remove(waitingTask);
                this.waitingTasksSemaphore.Release();

                return msg;
            }
        }


        public TResponse Wait<TRequest, TResponse>(TRequest request, TimeSpan timeout)
            where TRequest : MessageRequestBase
            where TResponse : MessageResponseBase<TRequest>, IMessageResponse
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            this.waitingTasksSemaphore.Wait();
            using (var waitingTask = new WaitingTask(request, typeof(TResponse)))
            {
                this.waitingTasks.Add(waitingTask);
                this.waitingTasksSemaphore.Release();

                var msg = waitingTask.Wait<TRequest, TResponse>(timeout);

                this.waitingTasksSemaphore.Wait();
                this.waitingTasks.Remove(waitingTask);
                this.waitingTasksSemaphore.Release();

                return msg;
            }
        }


        public async Task<TResponse> WaitAsync<TRequest, TResponse>(TRequest request)
            where TRequest : MessageRequestBase
            where TResponse : MessageResponseBase<TRequest>, IMessageResponse
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            await this.waitingTasksSemaphore.WaitAsync();
            using (var waitingTask = new WaitingTask(request, typeof(TResponse)))
            {
                this.waitingTasks.Add(waitingTask);
                this.waitingTasksSemaphore.Release();

                var msg = await waitingTask.WaitAsync<TRequest, TResponse>();

                await this.waitingTasksSemaphore.WaitAsync();
                this.waitingTasks.Remove(waitingTask);
                this.waitingTasksSemaphore.Release();

                return msg;
            }
        }

        // TODO: comment methods, maybe implement kind of TRsp Request<TRq, TRsp>(TRq) method

        public async Task<TResponse> WaitAsync<TRequest, TResponse>(TRequest request, TimeSpan timeout)
            where TRequest : MessageRequestBase
            where TResponse : MessageResponseBase<TRequest>, IMessageResponse
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            await this.waitingTasksSemaphore.WaitAsync();
            using (var waitingTask = new WaitingTask(request, typeof(TResponse)))
            {
                this.waitingTasks.Add(waitingTask);
                this.waitingTasksSemaphore.Release();

                var msg = await waitingTask.WaitAsync<TRequest, TResponse>(timeout);

                await this.waitingTasksSemaphore.WaitAsync();
                this.waitingTasks.Remove(waitingTask);
                this.waitingTasksSemaphore.Release();

                return msg;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (disposing)
                {
                    // remove event handler
                    this.receiver.MessageReceived -= this.Receiver_ReceivedMessage;
                }

                // dispose waiting tasks
                this.waitingTasksSemaphore.Wait();
                this.waitingTasks.ForEach(t => t.Dispose());
                this.waitingTasksSemaphore.Release();

                // dispose semaphore
                this.waitingTasksSemaphore.Dispose();
            }
        }

        /// <summary>
        /// Handles the <see cref="IMessageReceiver.MessageReceived" /> event of the <see cref="receiver" /> object.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the event.
        /// </param>
        /// <param name="e">
        ///     Contains additional information for the event handler.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="e" /> is <c>null</c>.</para>
        /// </exception>
        private async void Receiver_ReceivedMessage(object sender, MessageReceivedEventArgs e)
        {
            if (object.ReferenceEquals(e, null))
            {
                throw new ArgumentNullException(nameof(e));
            }

            this.messageHandlersSemaphore.Wait();
            try
            {
                this.messageHandlers.Where(h => h.MessageClrType.GetTypeInfo().IsAssignableFrom(e.ReceivedMessage.GetType().GetTypeInfo()))
                                    .ForEach(h => h.HandleMessage(e.ReceivedMessage));
            }
            finally
            {
                this.messageHandlersSemaphore.Release();
            }

            var rsp = e.ReceivedMessage as IMessageResponse;

            if (rsp != null)
            {
                await this.waitingTasksSemaphore.WaitAsync();
                this.waitingTasks.ForEach(t => t.PushMessage(rsp));
                this.waitingTasksSemaphore.Release();
            }
        }

        /// <summary>
        /// Describes a wrapper around a task waiting for a response message.
        /// </summary>
        /// <seealso cref="IDisposable" />
        private sealed class WaitingTask
            : IDisposable
        {
            /// <summary>
            /// The backing field for the <see cref="RequestMessage" /> property.
            /// </summary>
            private readonly MessageRequestBase requestMessageBacking;

            /// <summary>
            /// The <see cref="AutoResetEvent" /> wait handle blocking the <see cref="Wait{TRequest, TResponse}()" />
            /// and the <see cref="WaitAsync{TRequest, TResponse}" /> method.
            /// </summary>
            private readonly AutoResetEvent waitHandle;

            /// <summary>
            /// The <see cref="IMessageResponse" /> that has been received,
            /// or <c>null</c> if no response has been received yet.
            /// </summary>
            private IMessageResponse response;

            /// <summary>
            /// The <see cref="Type" /> representing the expected CLR type of the response.
            /// </summary>
            private readonly Type responseType;

            /// <summary>
            /// The <see cref="TypeInfo" /> representing the expected CLR type of the response.
            /// </summary>
            private readonly TypeInfo responseTypeInfo;

            /// <summary>
            /// Initializes a new instance of the <see cref="WaitingTask" /> class.
            /// </summary>
            /// <param name="request">
            ///     The request <see cref="MessageRequestBase" />.
            ///     Must not be <c>null</c>.
            /// </param>
            /// <exception cref="ArgumentNullException">
            ///     <para>Thrown if:</para>
            ///     <para>  - <paramref name="request" /> is <c>null</c>.</para>
            ///     <para>  - <paramref name="source" /> is <c>null</c>.</para>
            /// </exception>
            internal WaitingTask(MessageRequestBase request, Type responseType)
            {
                if (request == default(MessageRequestBase))
                {
                    throw new ArgumentNullException(nameof(request));
                }

                if (responseType == null)
                {
                    throw new ArgumentNullException(nameof(responseType));
                }

                if (!typeof(IMessageResponse).GetTypeInfo().IsAssignableFrom(responseType.GetTypeInfo()))
                {
                    throw new InvalidOperationException($"The specified type \"{responseType}\" does not implement from \"{nameof(IMessageResponse)}\".");
                }

                this.requestMessageBacking = request;
                this.waitHandle = new AutoResetEvent(false);
                this.responseType = responseType;
                this.responseTypeInfo = responseType.GetTypeInfo();
            }


            /// <summary>
            /// Finalizes an instance of the <see cref="WaitingTask" /> class.
            /// </summary>
            ~WaitingTask()
            {
                this.Dispose(false);
            }


            internal MessageRequestBase RequestMessage
            {
                get { return this.requestMessageBacking; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether the <see cref="WaitingTask" />is disposed.
            /// </summary>
            /// <value>
            ///     <c>true</c> if the <see cref="WaitingTask" /> is disposed; otherwise, <c>false</c>.
            /// </value>
            internal bool IsDisposed { get; private set; }

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


            internal TResponse Wait<TRequest, TResponse>()
                where TRequest : MessageRequestBase
                where TResponse : MessageResponseBase<TRequest>
            {
                if (!this.responseType.GetTypeInfo().IsAssignableFrom(typeof(TResponse).GetTypeInfo()))
                {
                    throw new InvalidMessageException();
                }

                this.waitHandle.WaitOne();
                var rsp = this.response as TResponse;
                if (rsp == null)
                {
                    throw new InvalidCastException();
                }

                return rsp;
            }


            internal TResponse Wait<TRequest, TResponse>(TimeSpan timeout)
                where TRequest : MessageRequestBase
                where TResponse : MessageResponseBase<TRequest>
            {
                if (!this.responseType.GetTypeInfo().IsAssignableFrom(typeof(TResponse).GetTypeInfo()))
                {
                    throw new InvalidMessageException();
                }

                if (!this.waitHandle.WaitOne(timeout))
                {
                    throw new TimeoutException();
                }

                var rsp = this.response as TResponse;
                if (rsp == null)
                {
                    throw new InvalidCastException();
                }

                return rsp;
            }


            internal async Task<TResponse> WaitAsync<TRequest, TResponse>()
                where TRequest : MessageRequestBase
                where TResponse : MessageResponseBase<TRequest>
            {
                if (!this.responseType.GetTypeInfo().IsAssignableFrom(typeof(TResponse).GetTypeInfo()))
                {
                    throw new InvalidMessageException();
                }

                await Task.Run(() => this.waitHandle.WaitOne());
                var rsp = this.response as TResponse;
                if (rsp == null)
                {
                    throw new InvalidCastException();
                }

                return rsp;
            }


            internal async Task<TResponse> WaitAsync<TRequest, TResponse>(TimeSpan timeout)
                where TRequest : MessageRequestBase
                where TResponse : MessageResponseBase<TRequest>
            {
                if (!this.responseType.GetTypeInfo().IsAssignableFrom(typeof(TResponse).GetTypeInfo()))
                {
                    throw new InvalidMessageException();
                }

                if (!await Task.Run(() => this.waitHandle.WaitOne(timeout)))
                {
                    throw new TimeoutException();
                }

                var rsp = this.response as TResponse;
                if (rsp == null)
                {
                    throw new InvalidCastException();
                }

                return rsp;
            }


            internal bool PushMessage(IMessageResponse response)
            {
                if (response == null)
                {
                    throw new ArgumentNullException(nameof(response));
                }

                var msgType = response.GetType().GetTypeInfo();

                if (!this.responseType.GetTypeInfo().IsAssignableFrom(msgType))
                {
                    return false;
                }

                if (response.RequestIdentifier == this.RequestMessage.Identifier)
                {
                    this.response = response;
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
