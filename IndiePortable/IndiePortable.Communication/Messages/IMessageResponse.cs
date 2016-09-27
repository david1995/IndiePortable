// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageResponse.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IMessageResponse interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Messages
{
    using System;
    using Formatter;

    /// <summary>
    /// Represents the non-generic base of response messages.
    /// </summary>
    [Serializable]
    public interface IMessageResponse
    {
        /// <summary>
        /// Gets the unique identifier of the request message.
        /// </summary>
        /// <value>
        ///     Contains the unique identifier of the request message.
        /// </value>
        Guid RequestIdentifier { get; }
    }
}
