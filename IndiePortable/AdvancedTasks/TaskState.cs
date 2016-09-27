// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskState.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the TaskState enum.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.AdvancedTasks
{
    /// <summary>
    /// Provides information about the state of a task.
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// Indicates that a <see cref="StateTask" /> has not been started.
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// Indicates that a <see cref="StateTask" /> has been started.
        /// </summary>
        Started = 1,

        /// <summary>
        /// Indicates that a <see cref="StateTask" /> has finished successfully.
        /// </summary>
        Finished = 2,

        /// <summary>
        /// Indicates that a <see cref="StateTask" /> has thrown an exception.
        /// </summary>
        ExceptionThrown = 3
    }
}
