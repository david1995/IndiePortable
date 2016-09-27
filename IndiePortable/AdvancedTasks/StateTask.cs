// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="StateTask.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the StateTask class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.AdvancedTasks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Wraps a <see cref="Task" /> that is destined to a long life span.
    /// </summary>
    public class StateTask
    {
        /// <summary>
        /// Represents the communication connection to the <see cref="StateTask" />.
        /// </summary>
        private readonly ITaskConnection connection;

        /// <summary>
        /// The <see cref="Task" /> that is wrapped by the <see cref="StateTask" />.
        /// </summary>
        private readonly Task task;

        /// <summary>
        /// The backing field for the <see cref="CurrentState" /> property.
        /// </summary>
        private TaskState currentStateBacking = TaskState.NotStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateTask" /> class.
        /// </summary>
        /// <param name="method">
        ///     The method the <see cref="StateTask" /> shall process.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="method" /> is <c>null</c>.</para>
        /// </exception>
        public StateTask(Action<ITaskConnection> method)
        {
            if (object.ReferenceEquals(method, null))
            {
                throw new ArgumentNullException(nameof(method));
            }

            this.connection = new TaskConnection(this);
            this.task = Task.Factory.StartNew(() => method(this.connection));
            this.currentStateBacking = TaskState.Started;
        }

        /// <summary>
        /// Raised when the <see cref="StateTask" /> has returned.
        /// </summary>
        public event EventHandler Returned;

        /// <summary>
        /// Raised when an <see cref="Exception" /> has been thrown during the execution of the <see cref="StateTask" />.
        /// </summary>
        public event EventHandler<ExceptionThrownEventArgs> ExceptionThrown;

        /// <summary>
        /// Gets the current state of the <see cref="StateTask" />.
        /// </summary>
        /// <value>
        ///     Contains the current state of the <see cref="StateTask" />.
        /// </value>
        public TaskState CurrentState => this.currentStateBacking;

        /// <summary>
        /// Signals the <see cref="StateTask" /> to stop.
        /// </summary>
        public void Stop() => this.connection.Stop();

        /// <summary>
        /// Signals the <see cref="StateTask" /> to stop and waits until the <see cref="StateTask" /> returns.
        /// If the <see cref="StateTask" /> has already finished, the call immediately returns.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If the <see cref="ITaskConnection.Return()" /> method or the
        ///         <see cref="ITaskConnection.ThrowException(Exception)" /> method is never called by the task method,
        ///         calls to the <see cref="StopAndAwait()" /> method will never return.
        ///     </para>
        /// </remarks>
        public void StopAndAwait()
        {
            this.connection.Stop();
            this.connection.Await();
        }

        /// <summary>
        /// Signals the <see cref="StateTask" /> to stop and waits until the <see cref="StateTask" /> returns asynchronously.
        /// If the <see cref="StateTask" /> has already finished, the call immediately returns.
        /// </summary>
        /// <returns>
        ///     Returns the executing <see cref="Task" />.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If the <see cref="ITaskConnection.Return()" /> method or the
        ///         <see cref="ITaskConnection.ThrowException(Exception)" /> method is never called by the task method,
        ///         calls to the <see cref="StopAndAwaitAsync()" /> method will never return.
        ///     </para>
        /// </remarks>
        public async Task StopAndAwaitAsync()
        {
            this.connection.Stop();
            await this.connection.AwaitAsync();
        }

        /// <summary>
        /// Waits until the <see cref="StateTask" /> returns.
        /// If the <see cref="StateTask" /> has already finished, the call immediately returns.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If the <see cref="ITaskConnection.Return()" /> method or the
        ///         <see cref="ITaskConnection.ThrowException(Exception)" /> method is never called by the task method,
        ///         calls to the <see cref="Await()" /> method will never return.
        ///     </para>
        /// </remarks>
        public void Await() => this.connection.Await();

        /// <summary>
        /// Waits until the <see cref="StateTask" /> returns asynchronously.
        /// If the <see cref="StateTask" /> has already finished, the call immediately returns.
        /// </summary>
        /// <returns>
        ///     Returns the executing <see cref="Task" />.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If the <see cref="ITaskConnection.Return()" /> method or the
        ///         <see cref="ITaskConnection.ThrowException(Exception)" /> method is never called by the task method,
        ///         calls to the <see cref="Await()" /> method will never return.
        ///     </para>
        /// </remarks>
        public Task AwaitAsync() => this.connection.AwaitAsync();
        
        /// <summary>
        /// Waits until the <see cref="StateTask" /> returns.
        /// If the <see cref="StateTask" /> has already finished, the call immediately returns.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if no exception has been thrown; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If the <see cref="ITaskConnection.Return()" /> method or the
        ///         <see cref="ITaskConnection.ThrowException(Exception)" /> method is never called by the task method,
        ///         calls to the <see cref="TryAwait()" /> method will never return.
        ///     </para>
        /// </remarks>
        public bool TryAwait() => this.connection.TryAwait();
        
        /// <summary>
        /// Waits until the <see cref="StateTask" /> returns asynchronously.
        /// If the <see cref="StateTask" /> has already finished, the call immediately returns.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if no exception has been thrown; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If the <see cref="ITaskConnection.Return()" /> method or the
        ///         <see cref="ITaskConnection.ThrowException(Exception)" /> method is never called by the task method,
        ///         calls to the <see cref="TryAwaitAsync()" /> method will never return.
        ///     </para>
        /// </remarks>
        public Task<bool> TryAwaitAsync() => this.connection.TryAwaitAsync();

        /// <summary>
        /// Raises the <see cref="Returned" /> event.
        /// </summary>
        protected void RaiseReturned() => this.Returned?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Raises the <see cref="ExceptionThrown" /> event.
        /// </summary>
        /// <param name="exc">
        ///     The <see cref="Exception" /> that has been thrown.
        /// </param>
        protected void RaiseExceptionThrown(Exception exc) => this.ExceptionThrown?.Invoke(this, new ExceptionThrownEventArgs(exc));

        /// <summary>
        /// Represents the connection to the method of the wrapped <see cref="Task" />.
        /// </summary>
        /// <seealso cref="ITaskConnection" />
        private sealed class TaskConnection
            : ITaskConnection
        {
            /// <summary>
            /// The <see cref="ManualResetEventSlim" /> used to block for the <see cref="Await()" /> method family.
            /// </summary>
            private readonly ManualResetEventSlim waitHandle = new ManualResetEventSlim();

            /// <summary>
            /// The <see cref="StateTask" /> using this <see cref="TaskConnection" /> instance.
            /// </summary>
            private readonly StateTask task;

            /// <summary>
            /// The backing field for the <see cref="MustFinish" /> property.
            /// </summary>
            private bool mustFinishBacking;

            /// <summary>
            /// The <see cref="Exception" /> that has been thrown, or <c>null</c> if no exception has been thrown.
            /// </summary>
            private Exception thrownException;

            /// <summary>
            /// Initializes a new instance of the <see cref="TaskConnection"/> class.
            /// </summary>
            /// <param name="task">
            ///     The <see cref="StateTask" /> using the <see cref="TaskConnection" /> instance.
            /// </param>
            /// <exception cref="ArgumentNullException">
            ///     <para>Thrown if <paramref name="task" /> is <c>null</c>.</para>
            /// </exception>
            public TaskConnection(StateTask task)
            {
                if (object.ReferenceEquals(task, null))
                {
                    throw new ArgumentNullException(nameof(task));
                }

                this.task = task;
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="StateTask" /> must finish.
            /// </summary>
            /// <value>
            ///     <c>true</c> if the <see cref="StateTask" /> must finish; otherwise <c>false</c>.
            /// </value>
            /// <remarks>
            ///     <para>Implements <see cref="ITaskConnection.MustFinish" /> implicitly.</para>
            /// </remarks>
            public bool MustFinish => this.mustFinishBacking;


            public void Stop()
            {
                this.mustFinishBacking = true;
            }


            public void Await()
            {
                this.waitHandle.Wait();
                if (this.task.CurrentState == TaskState.ExceptionThrown &&
                    !object.ReferenceEquals(this.thrownException, null))
                {
                    throw this.thrownException;
                }
            }


            public async Task AwaitAsync()
            {
                await Task.Factory.StartNew(() => this.waitHandle.Wait());
                if (this.task.CurrentState == TaskState.ExceptionThrown &&
                    !object.ReferenceEquals(this.thrownException, null))
                {
                    throw this.thrownException;
                }
            }


            public bool TryAwait()
            {
                this.waitHandle.Wait();
                return this.task.currentStateBacking == TaskState.Finished;
            }


            public async Task<bool> TryAwaitAsync()
            {
                await Task.Factory.StartNew(() => this.waitHandle.Wait());
                return this.task.currentStateBacking == TaskState.Finished;
            }

            /// <summary>
            /// Signals that the <see cref="StateTask" /> has finished its work.
            /// </summary>
            public void Return()
            {
                this.task.currentStateBacking = TaskState.Finished;
                this.mustFinishBacking = true;
                this.waitHandle.Set();
                this.task.RaiseReturned();
            }

            /// <summary>
            /// Notifies the <see cref="StateTask" /> of a thrown exception.
            /// </summary>
            /// <param name="exc">
            ///     The thrown <see cref="Exception" />.
            ///     Must not be <c>null</c>.
            /// </param>
            /// <exception cref="ArgumentNullException">
            ///     <para>Thrown if <paramref name="exc" /> is <c>null</c>.</para>
            /// </exception>
            public void ThrowException(Exception exc)
            {
                if (object.ReferenceEquals(exc, null))
                {
                    throw new ArgumentNullException(nameof(exc));
                }    

                this.task.currentStateBacking = TaskState.ExceptionThrown;
                this.mustFinishBacking = true;
                this.thrownException = exc;
                this.waitHandle.Set();
                this.task.RaiseExceptionThrown(exc);
            }
        }
    }
}
