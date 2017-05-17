// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageResponse.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IMessageResponse interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Messages
{
    using System;

    /// <summary>
    /// Represents the non-generic base of response messages.
    /// </summary>
    public interface IMessageResponse
    {
        /// <summary>
        /// Gets the unique identifier of the request message.
        /// </summary>
        /// <value>
        /// Contains the unique identifier of the request message.
        /// </value>
        Guid RequestIdentifier { get; }
    }
}
