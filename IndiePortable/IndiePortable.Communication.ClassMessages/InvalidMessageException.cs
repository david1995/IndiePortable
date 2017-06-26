// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidMessageException.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the InvalidMessageException class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.ClassMessages
{
    using System;

    /// <summary>
    /// Represents an error that occurs when a specified message does not fulfill some conditions.
    /// </summary>
    /// <seealso cref="Exception" />
    public class InvalidMessageException
       : Exception
    {
        /// <summary>
        /// The backing field for the <see cref="Reason" /> property.
        /// </summary>
        private readonly MessageBase reasonBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMessageException" /> class.
        /// </summary>
        public InvalidMessageException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMessageException" /> class.
        /// </summary>
        /// <param name="reason">
        ///     The <see cref="MessageBase" /> representing the reason for the error.
        /// </param>
        public InvalidMessageException(MessageBase reason)
        {
            this.reasonBacking = reason;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMessageException" /> class with a message.
        /// </summary>
        /// <param name="message">
        ///     The message that describes the error.
        /// </param>
        public InvalidMessageException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMessageException"/> class.
        /// </summary>
        /// <param name="message">
        ///     The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or <c>null</c> (Nothing in Visual Basic) if no inner exception is specified.
        /// </param>
        public InvalidMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets the <see cref="MessageBase" /> representing the reason for the error.
        /// </summary>
        /// <value>
        ///     Contains the <see cref="MessageBase" /> representing the reason for the error.
        /// </value>
        public MessageBase Reason => this.reasonBacking;
    }
}
