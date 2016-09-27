// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="StateTask.Generic.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the StateTask{T} class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.AdvancedTasks
{
    using System;
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
    [Obsolete("The StateTask<T> class is deprecated. Use the StateTask class instead.", true)]
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
        }
        
        /// <summary>
        /// Raised when the <see cref="StateTask{T}" /> has returned.
        /// </summary>
        public event EventHandler Returned
        {
            add { throw new NotImplementedException(); }

            remove { throw new NotImplementedException(); }
        }
        
        /// <summary> 
        /// Gets or sets the state object that is passed to the <see cref="StateTask{T}" />. 
        /// </summary> 
        /// <value> 
        ///     Contains the state object that is passed to the <see cref="StateTask{T}" />. 
        /// </value> 
        public T StateObject
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Signals the <see cref="StateTask{T}" /> to stop.
        /// </summary>
        public void Stop()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
        public Task StopAndWaitAsync()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
        public Task WaitForReturnAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary> 
        /// Raises the <see cref="Returned" /> event. 
        /// </summary> 
        protected void RaiseReturned() => this.Returned?.Invoke(this, EventArgs.Empty);
    }
}
