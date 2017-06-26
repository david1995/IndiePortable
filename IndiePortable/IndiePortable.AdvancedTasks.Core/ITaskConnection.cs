// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ITaskConnection.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ITaskConnection interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.AdvancedTasks.Core
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the connection to a task.
    /// </summary>
    public interface ITaskConnection
    {
        /// <summary>
        /// Occurs when a stop has been requested.
        /// </summary>
        event EventHandler StopRequested;

        /// <summary>
        /// Gets a value indicating whether the <see cref="StateTask" /> must finish.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="StateTask" /> must finish; otherwise <c>false</c>.
        /// </value>
        bool MustFinish { get; }

        /// <summary>
        /// Tells the <see cref="StateTask" /> to stop.
        /// </summary>
        void Stop();

        /// <summary>
        /// Waits until the <see cref="StateTask" /> returns.
        /// If the <see cref="StateTask" /> has already finished, the call immediately returns.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the <see cref="Return()" /> method or the <see cref="ThrowException(Exception)" /> method
        /// is never called by the task method,
        /// calls to the <see cref="Await()" /> method will never return.
        /// </para>
        /// </remarks>
        void Await();

        /// <summary>
        /// Waits until the <see cref="StateTask" /> returns asynchronously.
        /// If the <see cref="StateTask" /> has already finished, the call immediately returns.
        /// </summary>
        /// <returns>
        /// Returns the executing <see cref="Task" />.
        /// </returns>
        /// <remarks>
        /// <para>
        /// If the <see cref="Return()" /> method or the <see cref="ThrowException(Exception)" /> method
        /// is never called by the task method,
        /// calls to the <see cref="AwaitAsync()" /> method will never return.
        /// </para>
        /// </remarks>
        Task AwaitAsync();

        /// <summary>
        /// Waits until the <see cref="StateTask" /> returns.
        /// If the <see cref="StateTask" /> has already finished, the call immediately returns.
        /// </summary>
        /// <returns>
        /// <c>true</c> if no exception has been thrown; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// If the <see cref="Return()" /> method or the <see cref="ThrowException(Exception)" /> method
        /// is never called by the task method,
        /// calls to the <see cref="TryAwait()" /> method will never return.
        /// </para>
        /// </remarks>
        bool TryAwait();

        /// <summary>
        /// Waits until the <see cref="StateTask" /> returns.
        /// If the <see cref="StateTask" /> has already finished, the call immediately returns.
        /// </summary>
        /// <returns>
        /// <c>true</c> if no exception has been thrown; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// If the <see cref="Return()" /> method or the <see cref="ThrowException(Exception)" /> method
        /// is never called by the task method,
        /// calls to the <see cref="TryAwaitAsync()" /> method will never return.
        /// </para>
        /// </remarks>
        Task<bool> TryAwaitAsync();

        /// <summary>
        /// Signals that the <see cref="StateTask" /> has finished his work.
        /// </summary>
        void Return();

        /// <summary>
        /// Notifies the <see cref="StateTask" /> of a thrown exception.
        /// </summary>
        /// <param name="exc">
        /// The thrown <see cref="Exception" />.
        /// Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="exc" /> is <c>null</c>.</para>
        /// </exception>
        void ThrowException(Exception exc);
    }
}
