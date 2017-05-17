// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionMessageResponseBase.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionMessageResponseBase class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Core.Devices.ConnectionMessages
{
    using System;
    using System.Runtime.Serialization;
    using Formatter;

    /// <summary>
    /// Represents a connection response message.
    /// This class is abstract.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the request message.
    ///     Must derive from <see cref="ConnectionMessageRequestBase" />.
    /// </typeparam>
    /// <seealso cref="ConnectionMessageBase" />
    [Serializable]
    public abstract class ConnectionMessageResponseBase<T>
        : ConnectionMessageBase
        where T : ConnectionMessageRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMessageResponseBase{T}" /> class.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This constructor should not be called by a constructor of a deriving class
        ///         other than the parameterless constructor. It is used for de-serializing an object.
        ///     </para>
        /// </remarks>
        protected ConnectionMessageResponseBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMessageResponseBase{T}" /> class.
        /// </summary>
        /// <param name="request">
        ///     The request message that shall be responded to.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="request" /> is <c>null</c>.</para>
        /// </exception>
        protected ConnectionMessageResponseBase(T request)
        {
            this.RequestIdentifier = request?.MessageIdentifier ?? throw new ArgumentNullException(nameof(request));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMessageResponseBase{T}" /> class.
        /// </summary>
        /// <param name="data">
        /// The <see cref="SerializationInfo" /> that is used to populate the <see cref="ConnectionMessageResponseBase{T}" /> instance.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="ctx">
        /// The streaming context of the deserialization.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>Thrown if <paramref name="data" /> does not contain the necessary information.</para>
        /// </exception>
        protected ConnectionMessageResponseBase(SerializationInfo data, StreamingContext ctx)
            : base(data, ctx)
        {
            this.RequestIdentifier = (Guid)data.GetValue(nameof(this.RequestIdentifier), typeof(Guid));
        }

        /// <summary>
        /// Gets the unique identifier of the request message.
        /// </summary>
        /// <value>
        /// Contains the unique identifier of the request message.
        /// </value>
        public Guid RequestIdentifier { get; }

        /// <summary>
        /// Populates a specified <see cref="SerializationInfo" /> instance
        /// with data from the <see cref="ConnectionMessageResponseBase{T}" /> instance.
        /// </summary>
        /// <param name="data">
        /// The <see cref="SerializationInfo" /> that shall be populated.
        /// Must not be <c>null</c>.
        /// </param>
        /// <param name="ctx">
        /// The streaming context of the serialization.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        public override void GetObjectData(SerializationInfo data, StreamingContext ctx)
        {
            base.GetObjectData(data, ctx);

            data.AddValue(nameof(this.RequestIdentifier), this.RequestIdentifier);
        }
    }
}
