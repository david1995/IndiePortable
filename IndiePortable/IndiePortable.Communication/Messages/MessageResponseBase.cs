// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageResponseBase.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the MessageResponseBase&lt;T&gt; class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Messages
{
    using System;
    using Formatter;
    
    /// <summary>
    /// Represents a base class for all response message types. This class is abstract.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the request message.
    /// </typeparam>
    /// <seealso cref="MessageBase" />
    /// <seealso cref="IMessageResponse" />
    [Serializable]
    public abstract class MessageResponseBase<T>
        : MessageBase, IMessageResponse
        where T : MessageRequestBase
    {
        /// <summary>
        /// The backing field for the <see cref="RequestIdentifier" /> property.
        /// </summary>
        private readonly Guid requestIdentifierBacking;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageResponseBase{T}" /> class.
        /// </summary>
        protected MessageResponseBase()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageResponseBase{T}" /> class.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> populated with data for the <see cref="MessageResponseBase{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        protected MessageResponseBase(ObjectDataCollection data)
            : base(data)
        {
            this.requestIdentifierBacking = data.GetValue<Guid>(nameof(this.RequestIdentifier));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageResponse" /> class.
        /// </summary>
        /// <param name="requestIdentifier">
        ///     The <see cref="Guid" /> of the request <see cref="MessageBase" />.
        /// </param>
        protected MessageResponseBase(Guid requestIdentifier)
            : this(Guid.NewGuid(), requestIdentifier)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageResponseBase{T}"/> class.
        /// </summary>
        /// <param name="messageIdentifier">
        ///     The <see cref="Guid" /> identifying the <see cref="MessageResponseBase{T}" />.
        /// </param>
        /// <param name="requestIdentifier">
        ///     The <see cref="Guid" /> of the request <see cref="MessageBase" />.
        /// </param>
        protected MessageResponseBase(Guid messageIdentifier, Guid requestIdentifier)
            : base(messageIdentifier)
        {
            this.requestIdentifierBacking = requestIdentifier;
        }

        /// <summary>
        /// Gets the unique identifier of the request <see cref="MessageBase" />.
        /// </summary>
        /// <value>
        ///     Contains the unique identifier of the request <see cref="MessageBase" />.
        /// </value>
        /// <remarks>
        ///     <para>Implements <see cref="IMessageResponse.RequestIdentifier" /> implicitly.</para>
        /// </remarks>
        public Guid RequestIdentifier => this.requestIdentifierBacking;

        /// <summary>
        /// Populates a specified <see cref="ObjectDataCollection" /> instance with data from the <see cref="MessageResponseBase{T}" /> instance.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> that shall be populated.
        /// </param>
        /// <remarks>
        ///     <para>Overrides <see cref="MessageBase.GetObjectData(ObjectDataCollection)" />.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <seealso cref="MessageBase.GetObjectData(ObjectDataCollection)" />
        /// <seealso cref="ISerializable.GetObjectData(ObjectDataCollection)" />
        /// <seealso cref="ISerializable" />
        public override void GetObjectData(ObjectDataCollection data)
        {
            base.GetObjectData(data);

            data.AddValue(nameof(this.RequestIdentifier), this.RequestIdentifier);
        }
    }
}
