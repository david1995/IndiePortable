// <copyright file="MessageDispatcher{TMessage}.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndiePortable.Collections.Linq;
using IndiePortable.Communication.Core;

namespace IndiePortable.Communication.Message
{
    /// <summary>
    /// Provides simplified methods for handling incoming messages and message response waiting.
    /// </summary>
    /// <typeparam name="TMessage">The type of the messages. Must be a reference type.</typeparam>
    /// <seealso cref="IDisposable" />
    public class MessageDispatcher<TMessage>
        : IDisposable
        where TMessage : class
    {
        /// <summary>
        /// The <see cref="SemaphoreSlim" /> controlling the thread access to <see cref="waitingTasks" />.
        /// </summary>
        private readonly SemaphoreSlim waitingTasksSemaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// The list containing all currently waiting message response tasks.
        /// </summary>
        private readonly List<WaitingTask> waitingTasks = new List<WaitingTask>();
        private readonly List<IMessageHandler<TMessage>> messageHandlers;
        private readonly SemaphoreSlim messageHandlersSemaphore = new SemaphoreSlim(1, 1);
        private readonly Guid signature;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDispatcher{TMessage}" /> class.
        /// </summary>
        /// <param name="signature">
        /// The contract signature of the connection.
        /// </param>
        public MessageDispatcher(Guid signature)
        {
            this.signature = signature;

            this.messageHandlers = new List<IMessageHandler<TMessage>>();
            this.MessageHandlers = new ReadOnlyCollection<IMessageHandler<TMessage>>(this.messageHandlers);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MessageDispatcher{TMessage}" /> class.
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
        public IReadOnlyCollection<IMessageHandler<TMessage>> MessageHandlers { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="MessageDispatcher" /> is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="MessageDispatcher" /> is disposed; otherwise <c>false</c>.
        /// </value>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Registers a message handler at the <see cref="MessageDispatcher" />.
        /// </summary>
        /// <param name="messageHandler">
        /// The <see cref="IMessageHandler{TMessage}" /> that shall be registered.
        /// Must not be <c>null</c>-
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="messageHandler" /> is <c>null</c>.</para>
        /// </exception>
        public void AddMessageHandler(IMessageHandler<TMessage> messageHandler)
        {
            if (messageHandler is null)
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
        /// The <see cref="IMessageHandler" /> that shall be unregistered.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="messageHandler" /> is <c>null</c>-</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="messageHandler" /> is not registered at the <see cref="MessageDispatcher{TMessage}" />.
        /// </exception>
        public void RemoveMessageHandler(IMessageHandler<TMessage> messageHandler)
        {
            if (messageHandler is null)
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
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Waits for the response to a specified request message.
        /// </summary>
        /// <param name="request">
        /// The request message whose response shall be awaited.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="determinator">
        /// Determines which message represents the response.
        /// Must not be <c>null</c>.
        /// </param>
        /// <returns>
        /// Returns the response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <list type="bullet">
        /// <item><paramref name="request"/> is <c>null</c>.</item>
        /// <item><paramref name="determinator"/> is <c>null</c>.</item>
        /// </list>
        /// </exception>
        public TMessage Wait(TMessage request, Predicate<TMessage> determinator)
        {
            using (var waitingTask = new WaitingTask(
                request ?? throw new ArgumentNullException(nameof(request)),
                determinator ?? throw new ArgumentNullException(nameof(determinator))))
            {
                this.waitingTasksSemaphore.Wait();
                try
                {
                    this.waitingTasks.Add(waitingTask);
                }
                finally
                {
                    this.waitingTasksSemaphore.Release();
                }

                var msg = waitingTask.Wait();

                this.waitingTasksSemaphore.Wait();
                try
                {
                    this.waitingTasks.Remove(waitingTask);
                }
                finally
                {
                    this.waitingTasksSemaphore.Release();
                }

                return msg;
            }
        }

        /// <summary>
        /// Waits for the response to a specified request message with a given maximum timeout.
        /// </summary>
        /// <typeparam name="TRequest">
        /// The type of the request message.
        /// Must derive from <see cref="MessageRequestBase" />.
        /// </typeparam>
        /// <typeparam name="TResponse">
        /// The type of the response message.
        /// Must derive from <see cref="MessageResponseBase{T}" /> and implement <see cref="IMessageResponse" />.
        /// </typeparam>
        /// <param name="request">
        /// The request message whose response shall be awaited.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="determinator">
        /// Determines which message represents the response.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="timeout">
        /// The maximum time span that shall be waited for the response.
        /// </param>
        /// <returns>
        /// Returns the response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <list type="bullet">
        /// <item><paramref name="request"/> is <c>null</c>.</item>
        /// <item><paramref name="determinator"/> is <c>null</c>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para>Thrown if <paramref name="timeout" /> is smaller than <see cref="TimeSpan.Zero" />.</para>
        /// </exception>
        public TMessage Wait(TMessage request, Predicate<TMessage> determinator, TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }

            using (var waitingTask = new WaitingTask(
                request ?? throw new ArgumentNullException(nameof(request)),
                determinator ?? throw new ArgumentNullException(nameof(determinator))))
            {
                this.waitingTasksSemaphore.Wait();
                try
                {
                    this.waitingTasks.Add(waitingTask);
                }
                finally
                {
                    this.waitingTasksSemaphore.Release();
                }

                var msg = waitingTask.Wait(timeout);

                this.waitingTasksSemaphore.Wait();
                try
                {
                    this.waitingTasks.Remove(waitingTask);
                }
                finally
                {
                    this.waitingTasksSemaphore.Release();
                }

                return msg;
            }
        }

        /// <summary>
        /// Asynchonously waits for the response to a specified request message.
        /// </summary>
        /// <typeparam name="TRequest">
        /// The type of the request message.
        /// Must derive from <see cref="MessageRequestBase" />.
        /// </typeparam>
        /// <typeparam name="TResponse">
        /// The type of the response message.
        /// Must derive from <see cref="MessageResponseBase{T}" /> and implement <see cref="IMessageResponse" />.
        /// </typeparam>
        /// <param name="request">
        /// The request message whose response shall be awaited.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="determinator">
        /// Determines which message represents the response.
        /// Must not be <c>null</c>.
        /// </param>
        /// <returns>
        /// The task waiting for the response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <list type="bullet">
        /// <item><paramref name="request"/> is <c>null</c>.</item>
        /// <item><paramref name="determinator"/> is <c>null</c>.</item>
        /// </list>
        /// </exception>
        public async Task<TMessage> WaitAsync(TMessage request, Predicate<TMessage> determinator)
        {
            using (var waitingTask = new WaitingTask(
                request ?? throw new ArgumentNullException(nameof(request)),
                determinator ?? throw new ArgumentNullException(nameof(determinator))))
            {
                this.waitingTasksSemaphore.Wait();
                try
                {
                    this.waitingTasks.Add(waitingTask);
                }
                finally
                {
                    this.waitingTasksSemaphore.Release();
                }

                var msg = await waitingTask.WaitAsync();

                this.waitingTasksSemaphore.Wait();
                try
                {
                    this.waitingTasks.Remove(waitingTask);
                }
                finally
                {
                    this.waitingTasksSemaphore.Release();
                }

                return msg;
            }
        }

        /// <summary>
        /// Asynchronously waits for the response to a specified request message with a given maximum timeout.
        /// </summary>
        /// <typeparam name="TRequest">
        /// The type of the request message.
        /// Must derive from <see cref="MessageRequestBase" />.
        /// </typeparam>
        /// <typeparam name="TResponse">
        /// The type of the response message.
        /// Must derive from <see cref="MessageResponseBase{T}" /> and implement <see cref="IMessageResponse" />.
        /// </typeparam>
        /// <param name="request">
        /// The request message whose response shall be awaited.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="determinator">
        /// Determines which message represents the response.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="timeout">
        /// The maximum time span that shall be waited for the response.
        /// </param>
        /// <returns>
        /// The task waiting for the response message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <list type="bullet">
        /// <item><paramref name="request"/> is <c>null</c>.</item>
        /// <item><paramref name="determinator"/> is <c>null</c>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para>Thrown if <paramref name="timeout" /> is smaller than <see cref="TimeSpan.Zero" />.</para>
        /// </exception>
        public async Task<TMessage> WaitAsync(TMessage request, Predicate<TMessage> determinator, TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }

            using (var waitingTask = new WaitingTask(
                request ?? throw new ArgumentNullException(nameof(request)),
                determinator ?? throw new ArgumentNullException(nameof(determinator))))
            {
                this.waitingTasksSemaphore.Wait();
                try
                {
                    this.waitingTasks.Add(waitingTask);
                }
                finally
                {
                    this.waitingTasksSemaphore.Release();
                }

                var msg = await waitingTask.WaitAsync(timeout);

                this.waitingTasksSemaphore.Wait();
                try
                {
                    this.waitingTasks.Remove(waitingTask);
                }
                finally
                {
                    this.waitingTasksSemaphore.Release();
                }

                return msg;
            }
        }

        public async void PushMessage(TMessage message, Guid signature)
        {
            if (signature != this.signature)
            {
                throw new InvalidOperationException("The signature must match the signature passed at initialization.");
            }

            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.messageHandlersSemaphore.Wait();
            try
            {
                this.messageHandlers.Where(h => h.CanHandleMessage(message))
                                    .ForEach(h => h.HandleMessage(message));
            }
            finally
            {
                this.messageHandlersSemaphore.Release();
            }

            await this.waitingTasksSemaphore.WaitAsync();
            try
            {
                this.waitingTasks.Where(t => t.CanPushMessage(message))
                                    .ForEach(t => t.PushMessage(message));
            }
            finally
            {
                this.waitingTasksSemaphore.Release();
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (disposing)
                {
                }

                // dispose waiting tasks
                this.waitingTasksSemaphore.Wait();
                this.waitingTasks.Where(t => !t.IsDisposed).ForEach(t => t.Dispose());
                this.waitingTasks.Clear();

                // dispose semaphore
                this.waitingTasksSemaphore.Dispose();

                this.messageHandlersSemaphore.Wait();
                this.messageHandlersSemaphore.Dispose();

                this.IsDisposed = true;
            }
        }

        /// <summary>
        /// Describes a wrapper around a task waiting for a response message.
        /// </summary>
        /// <seealso cref="IDisposable" />
        private sealed class WaitingTask
            : IDisposable
        {
            private readonly AutoResetEvent waitHandle;
            private readonly Predicate<TMessage> determinator;
            private TMessage response;

            /// <summary>
            /// Initializes a new instance of the <see cref="WaitingTask" /> class.
            /// </summary>
            /// <param name="request">
            /// The request <see cref="MessageRequestBase" />.
            /// Must not be <c>null</c>.
            /// </param>
            /// <param name="determinator">
            /// Determines whether a message can be handled.
            /// Must not be <c>null</c>.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// <list type="bullet">
            /// <item><paramref name="request"/> is <c>null</c>.</item>
            /// <item><paramref name="determinator"/> is <c>null</c>.</item>
            /// </list>
            /// </exception>
            internal WaitingTask(TMessage request, Predicate<TMessage> determinator)
            {
                this.RequestMessage = request ?? throw new ArgumentNullException(nameof(request));
                this.determinator = determinator ?? throw new ArgumentNullException(nameof(determinator));
                this.waitHandle = new AutoResetEvent(false);
            }

            /// <summary>
            /// Finalizes an instance of the <see cref="WaitingTask" /> class.
            /// </summary>
            ~WaitingTask()
            {
                this.Dispose(false);
            }

            internal TMessage RequestMessage { get; }

            /// <summary>
            /// Gets or sets a value indicating whether the <see cref="WaitingTask" />is disposed.
            /// </summary>
            /// <value>
            /// <c>true</c> if the <see cref="WaitingTask" /> is disposed; otherwise, <c>false</c>.
            /// </value>
            internal bool IsDisposed { get; private set; }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <remarks>
            /// <para>Implements <see cref="IDisposable.Dispose()" /> implicitly.</para>
            /// </remarks>
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            internal TMessage Wait()
            {
                this.waitHandle.WaitOne();
                return this.response;
            }

            internal TMessage Wait(TimeSpan timeout)
                => !this.waitHandle.WaitOne(timeout)
                 ? throw new TimeoutException()
                 : this.response;

            internal async Task<TMessage> WaitAsync()
            {
                await Task.Run(() => this.waitHandle.WaitOne());
                return this.response;
            }

            internal async Task<TMessage> WaitAsync(TimeSpan timeout)
                => !(await Task.Run(() => this.waitHandle.WaitOne(timeout)))
                 ? throw new TimeoutException()
                 : this.response;

            internal bool CanPushMessage(TMessage message)
                => this.determinator(message);

            internal void PushMessage(TMessage message)
            {
                if (!this.CanPushMessage(message))
                {
                    throw new ArgumentException(
                        "The specified message cannot be pushed.",
                        nameof(message));
                }

                this.response = message;
                this.waitHandle.Set();
            }

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources.
            /// </summary>
            /// <param name="disposing">
            /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
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
