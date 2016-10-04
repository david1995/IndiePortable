// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRequestBase.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the MessageRequestBase class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Messages
{
    using System;
    using Formatter;

    /// <summary>
    /// Represents the base class for request messages.
    /// This class is abstract.
    /// </summary>
    /// <seealso cref="MessageBase" />
    [Serializable]
    public abstract class MessageRequestBase
        : MessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRequestBase" /> class.
        /// </summary>
        /// <remarks>
        ///     <para>This constructor is only intended for formatters to create an empty <see cref="MessageRequestBase" />.</para>
        /// </remarks>
        protected MessageRequestBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRequestBase"/> class.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> populated with data for the <see cref="MessageBase" />.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        protected MessageRequestBase(ObjectDataCollection data)
            : base(data)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRequestBase"/> class.
        /// </summary>
        /// <param name="messageIdentifier">
        ///     The identifier of the <see cref="MessageRequestBase" />.
        /// </param>
        protected MessageRequestBase(Guid messageIdentifier)
            : base(messageIdentifier)
        {
        }
    }
}
