// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionThrownEventArgs.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ExceptionThrownEventArgs class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.AdvancedTasks.Core
{
    using System;

    /// <summary>
    /// Provides event information for an event concerning a thrown exception.
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class ExceptionThrownEventArgs
        : EventArgs
    {
        /// <summary>
        /// The backing field for the <see cref="ThrownException" /> property.
        /// </summary>
        private readonly Exception thrownExceptionBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionThrownEventArgs" /> class.
        /// </summary>
        /// <param name="thrownException">
        ///     The <see cref="Exception" /> that has been thrown.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="thrownException" /> is <c>null</c>.</para>
        /// </exception>
        public ExceptionThrownEventArgs(Exception thrownException)
        {
            if (object.ReferenceEquals(thrownException, null))
            {
                throw new ArgumentNullException(nameof(thrownException));
            }

            this.thrownExceptionBacking = thrownException;
        }

        /// <summary>
        /// Gets the <see cref="Exception" /> that has been thrown.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="Exception" /> that has been thrown.
        /// </value>
        public Exception ThrownException => this.thrownExceptionBacking;
    }
}
