// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ITaskConnection.Generic.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contians the ITaskConnection{T} interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.AdvancedTasks
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the connection to a <see cref="StateTask{T}" />.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the state object that can be passed to the <see cref="StateTask{T}" />.
    /// </typeparam>
    /// <remarks>
    ///     <para>
    ///         The <see cref="ITaskConnection{T}" /> interface is deprecated. Use the <see cref="ITaskConnection" /> interface instead.
    ///     </para>
    /// </remarks>
    [Obsolete("The ITaskConnection<T> interface is deprecated. Use ITaskConnection instead.", true)]
    public interface ITaskConnection<T>
    {
        /// <summary>
        /// Gets a value indicating whether the <see cref="StateTask{T}" /> must finish.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="StateTask{T}" /> must finish; otherwise <c>false</c>.
        /// </value>
        [Obsolete("Deprecated. See ITaskConnection<T>.", true)]
        bool MustFinish { get; }

        /// <summary>
        /// Gets the state object passed to the <see cref="StateTask{T}" />.
        /// </summary>
        /// <value>
        ///     Contains the state object passed to the <see cref="StateTask{T}" />.
        /// </value>
        [Obsolete("Deprecated. See ITaskConnection<T>.", true)]
        T StateObject { get; }

        /// <summary>
        /// Tells the <see cref="StateTask{T}" /> to stop.
        /// </summary>
        [Obsolete("Deprecated. See ITaskConnection<T>.", true)]
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
        [Obsolete("Deprecated. See ITaskConnection<T>.", true)]
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
        [Obsolete("Deprecated. See ITaskConnection<T>.", true)]
        Task WaitForReturnAsync();

        /// <summary>
        /// Signals that the <see cref="StateTask{T}" /> has finished his work.
        /// </summary>
        [Obsolete("Deprecated. See ITaskConnection<T>.", true)]
        void Return();
    }
}
