// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="StateTask.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the StateTask class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace AdvancedTasks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a task that provides a state for communicating with the task.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the state object.
    /// </typeparam>
    /// <remarks>
    ///     <para>
    ///         This task is only recommended for tasks that are expected to have a long active period of time.
    ///         For tasks with a short active period, the direct usage of the <see cref="Task" /> class is more suitable.
    ///     </para>
    /// </remarks>
    public class StateTask<T>
    {
        /// <summary>
        /// Represents the communication connection to the <see cref="StateTask{T}" />.
        /// </summary>
        private readonly ITaskConnection<T> connection;

        /// <summary>
        /// The <see cref="Task" /> that is wrapped by the <see cref="StateTask{T}" />.
        /// </summary>
        private readonly Task task;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateTask{T}" /> class.
        /// </summary>
        /// <param name="method">
        ///     The method the <see cref="StateTask{T}" /> shall process.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="method" /> is <c>null</c>.</para>
        /// </exception>
        public StateTask(Action<ITaskConnection<T>> method)
        {
            if (object.ReferenceEquals(method, null))
            {
                throw new ArgumentNullException(nameof(method));
            }

            this.connection = new TaskConnection(this);
            this.task = Task.Factory.StartNew(() => method(this.connection));
            this.task.GetAwaiter().OnCompleted(() => this.RaiseReturned());
        }

        /// <summary>
        /// Raised when the <see cref="StateTask{T}" /> has returned.
        /// </summary>
        public event EventHandler Returned;

        /// <summary>
        /// Gets or sets the state object that is passed to the <see cref="StateTask{T}" />.
        /// </summary>
        /// <value>
        ///     Contains the state object that is passed to the <see cref="StateTask{T}" />.
        /// </value>
        public T StateObject { get; set; }

        /// <summary>
        /// Signals the <see cref="StateTask{T}" /> to stop.
        /// </summary>
        public void Stop()
        {
            this.connection.Stop();
        }

        /// <summary>
        /// Signals the <see cref="StateTask{T}" /> to stop and waits until the <see cref="StateTask{T}" /> returns.
        /// If the <see cref="StateTask{T}" /> has already finished, the call immediately returns.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If the <see cref="ITaskConnection{T}.Return()" /> method is never called by the task method,
        ///         calls to the <see cref="StopAndWait()" /> method will never return.
        ///     </para>
        /// </remarks>
        public void StopAndWait()
        {
            this.connection.Stop();
            this.connection.WaitForReturn();
        }

        /// <summary>
        /// Signals the <see cref="StateTask{T}" /> to stop and waits until the <see cref="StateTask{T}" /> returns asynchronously.
        /// If the <see cref="StateTask{T}" /> has already finished, the call immediately returns.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If the <see cref="ITaskConnection{T}.Return()" /> method is never called by the task method,
        ///         calls to the <see cref="StopAndWaitAsync()" /> method will never return.
        ///     </para>
        /// </remarks>
        public async Task StopAndWaitAsync()
        {
            this.connection.Stop();
            await this.connection.WaitForReturnAsync();
        }

        /// <summary>
        /// Waits until the <see cref="StateTask{T}" /> returns.
        /// If the <see cref="StateTask{T}" /> has already finished, the call immediately returns.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If the <see cref="ITaskConnection{T}.Return()" /> method is never called by the task method,
        ///         calls to the <see cref="WaitForReturn()" /> method will never return.
        ///     </para>
        /// </remarks>
        public void WaitForReturn()
        {
            this.connection.WaitForReturn();
        }

        /// <summary>
        /// Waits until the <see cref="StateTask{T}" /> returns asynchronously.
        /// If the <see cref="StateTask{T}" /> has already finished, the call immediately returns.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If the <see cref="ITaskConnection{T}.Return()" /> method is never called by the task method,
        ///         calls to the <see cref="WaitForReturnAsync()" /> method will never return.
        ///     </para>
        /// </remarks>
        public async Task WaitForReturnAsync()
        {
            await this.connection.WaitForReturnAsync();
        }

        /// <summary>
        /// Raises the <see cref="Returned" /> event.
        /// </summary>
        protected void RaiseReturned() => this.Returned?.Invoke(this, EventArgs.Empty);


        private sealed class TaskConnection
            : ITaskConnection<T>
        {

            private readonly ManualResetEventSlim waitHandle = new ManualResetEventSlim();


            private readonly StateTask<T> task;

            /// <summary>
            /// The backing field for the <see cref="StateObject" /> property.
            /// </summary>
            private T stateBacking;

            /// <summary>
            /// The backing field for the <see cref="HasFinished" /> property.
            /// </summary>
            private bool hasFinishedBacking;


            public TaskConnection(StateTask<T> task)
            {
                if (object.ReferenceEquals(task, null))
                {
                    throw new ArgumentNullException(nameof(task));
                }

                this.task = task;
            }

            /// <summary>
            /// Gets the state object passed to the <see cref="StateTask{T}" />.
            /// </summary>
            /// <value>
            ///     Contains the state object passed to the <see cref="StateTask{T}" />.
            /// </value>
            /// <remarks>
            ///     <para>Implements <see cref="ITaskConnection{T}.StateObject" /> implicitly.</para>
            /// </remarks>
            public T StateObject => this.task.StateObject;

            /// <summary>
            /// Gets a value indicating whether the <see cref="StateTask{T}" /> has finished.
            /// </summary>
            /// <value>
            ///     <c>true</c> if the <see cref="StateTask{T}" /> has finished; otherwise <c>false</c>.
            /// </value>
            /// <remarks>
            ///     <para>Implements <see cref="ITaskConnection{T}.HasFinished" /> implicitly.</para>
            /// </remarks>
            public bool HasFinished => this.hasFinishedBacking;

            
            public void Stop()
            {
                this.hasFinishedBacking = true;
            }


            public void WaitForReturn()
            {
                this.waitHandle.Wait();
            }


            public async Task WaitForReturnAsync()
            {
                await Task.Factory.StartNew(() => this.waitHandle.Wait());
            }


            public void Return()
            {
                this.waitHandle.Set();
                this.hasFinishedBacking = true;
                this.task.RaiseReturned();
            }
        }
    }
}
