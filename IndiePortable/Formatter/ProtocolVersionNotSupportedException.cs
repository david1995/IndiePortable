// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtocolVersionNotSupportedException.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ProtocolVersionNotSupportedException class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Formatter
{
    using System;

    /// <summary>
    /// Represents a runtime error concerning a not supported protocol version.
    /// </summary>
    /// <remarks>
    ///     Derives from <see cref="Exception" />.
    /// </remarks>
    public class ProtocolVersionNotSupportedException
        : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolVersionNotSupportedException" /> class.
        /// </summary>
        public ProtocolVersionNotSupportedException()
            : base("The protocol version is not supported.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolVersionNotSupportedException" /> class.
        /// </summary>
        /// <param name="message">
        ///     A message that describes the error.
        /// </param>
        public ProtocolVersionNotSupportedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolVersionNotSupportedException" /> class.
        /// </summary>
        /// <param name="message">
        ///     A message that describes the error.
        /// </param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a <c>null</c> reference
        ///     (Nothing in Visual Basic) if no inner exception is specified.
        /// </param>
        public ProtocolVersionNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolVersionNotSupportedException" /> class.
        /// </summary>
        /// <param name="protocolVersion">
        ///     The protocol <see cref="Version" /> that caused the error.
        /// </param>
        public ProtocolVersionNotSupportedException(Version protocolVersion)
            : this()
        {
            this.ProtocolVersion = protocolVersion;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolVersionNotSupportedException" /> class.
        /// </summary>
        /// <param name="message">
        ///     A message that describes the error.
        /// </param>
        /// <param name="protocolVersion">
        ///     The protocol <see cref="Version" /> that caused the error.
        /// </param>
        public ProtocolVersionNotSupportedException(string message, Version protocolVersion)
            : this(message)
        {
            this.ProtocolVersion = protocolVersion;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolVersionNotSupportedException"/> class.
        /// </summary>
        /// <param name="message">
        ///     A message that describes the error.
        /// </param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a <c>null</c> reference
        ///     (Nothing in Visual Basic) if no inner exception is specified.
        /// </param>
        /// <param name="protocolVersion">
        ///     The protocol <see cref="Version" /> that caused the error.
        /// </param>
        public ProtocolVersionNotSupportedException(string message, Exception innerException, Version protocolVersion)
            : this(message, innerException)
        {
            this.ProtocolVersion = protocolVersion;
        }

        /// <summary>
        /// Gets the protocol <see cref="Version" /> that caused the error.
        /// </summary>
        /// <value>
        ///     Contains the protocol <see cref="Version" /> that caused the error.
        /// </value>
        public Version ProtocolVersion { get; private set; }
    }
}
