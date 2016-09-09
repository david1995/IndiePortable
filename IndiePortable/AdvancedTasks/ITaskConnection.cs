// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ITaskConnection.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contians the ITaskConnection interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.AdvancedTasks
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the connection to a <see cref="StateTask{T}" />.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the state object that can be passed to the <see cref="StateTask{T}" />.
    /// </typeparam>
    public interface ITaskConnection<T>
    {
        /// <summary>
        /// Gets a value indicating whether the <see cref="StateTask{T}" /> has finished.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="StateTask{T}" /> has finished; otherwise <c>false</c>.
        /// </value>
        bool HasFinished { get; }

        /// <summary>
        /// Gets the state object passed to the <see cref="StateTask{T}" />.
        /// </summary>
        /// <value>
        ///     Contains the state object passed to the <see cref="StateTask{T}" />.
        /// </value>
        T StateObject { get; }

        /// <summary>
        /// Tells the <see cref="StateTask{T}" /> to stop.
        /// </summary>
        void Stop();

        /// <summary>
        /// Waits until the <see cref="StateTask{T}" /> returns.
        /// If the <see cref="StateTask{T}" /> has already finished, the call immediately returns.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If the <see cref="Return()" /> method is never called by the task method,
        ///         calls to the <see cref="WaitForReturn()" /> method will never return.
        ///     </para>
        /// </remarks>
        void WaitForReturn();

        /// <summary>
        /// Waits until the <see cref="StateTask{T}" /> returns asynchronously.
        /// If the <see cref="StateTask{T}" /> has already finished, the call immediately returns.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If the <see cref="Return()" /> method is never called by the task method,
        ///         calls to the <see cref="WaitForReturnAsync()" /> method will never return.
        ///     </para>
        /// </remarks>
        Task WaitForReturnAsync();

        /// <summary>
        /// Signals that the <see cref="StateTask{T}" /> has finished his work.
        /// </summary>
        void Return();
    }
}
