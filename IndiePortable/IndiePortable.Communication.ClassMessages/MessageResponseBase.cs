// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageResponseBase.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the MessageResponseBase&lt;T&gt; class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.ClassMessages
{
    using System;
    using System.Runtime.Serialization;

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
        /// The <see cref="SerializationInfo" /> populated with data for the <see cref="MessageResponseBase{T}" />.
        /// </param
        /// <param name="ctx">
        /// The streaming context for the deserialization.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        protected MessageResponseBase(SerializationInfo data, StreamingContext ctx)
            : base(data, ctx)
        {
            this.RequestIdentifier = (Guid)data.GetValue(nameof(this.RequestIdentifier), typeof(Guid));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageResponseBase{T}" /> class.
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
            this.RequestIdentifier = requestIdentifier;
        }

        /// <summary>
        /// Gets the unique identifier of the request <see cref="MessageBase" />.
        /// </summary>
        /// <value>
        /// Contains the unique identifier of the request <see cref="MessageBase" />.
        /// </value>
        public Guid RequestIdentifier { get; }

        /// <summary>
        /// Populates a specified <see cref="SerializationInfo" /> instance with data from the <see cref="MessageResponseBase{T}" /> instance.
        /// </summary>
        /// <param name="data">
        /// The <see cref="SerializationInfo" /> that shall be populated.
        /// </param>
        /// <remarks>
        /// <para>Overrides <see cref="MessageBase.GetObjectData(SerializationInfo)" />.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <seealso cref="MessageBase.GetObjectData(SerializationInfo, StreamingContext)" />
        /// <seealso cref="ISerializable.GetObjectData(SerializationInfo, StreamingContext)" />
        /// <seealso cref="ISerializable" />
        public override void GetObjectData(SerializationInfo data, StreamingContext ctx)
        {
            base.GetObjectData(data, ctx);
            data.AddValue(nameof(this.RequestIdentifier), this.RequestIdentifier);
        }
    }
}
