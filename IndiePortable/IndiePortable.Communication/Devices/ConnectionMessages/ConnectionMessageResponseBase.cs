// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionMessageResponseBase.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionMessageResponseBase class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices.ConnectionMessages
{
    using System;
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
        /// The backing field for the <see cref="RequestIdentifier" /> property.
        /// </summary>
        private readonly Guid requestIdentifierBacking;

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
            if (object.ReferenceEquals(request, null))
            {
                throw new ArgumentNullException(nameof(request));
            }

            this.requestIdentifierBacking = request.MessageIdentifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMessageResponseBase{T}" /> class.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> that is used
        ///     to populate the <see cref="ConnectionMessageResponseBase{T}" /> instance.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <para>Thrown if <paramref name="data" /> does not contain the necessary information.</para>
        /// </exception>
        protected ConnectionMessageResponseBase(ObjectDataCollection data)
            : base(data)
        {
            if (!data.TryGetValue(nameof(this.RequestIdentifier), out this.requestIdentifierBacking))
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Gets the unique identifier of the request message.
        /// </summary>
        /// <value>
        ///     Contains the unique identifier of the request message.
        /// </value>
        public Guid RequestIdentifier => this.requestIdentifierBacking;

        /// <summary>
        /// Populates a specified <see cref="ObjectDataCollection" /> instance
        /// with data from the <see cref="ConnectionMessageResponseBase{T}" /> instance.
        /// </summary>
        /// <param name="data">
        ///     The <see cref="ObjectDataCollection" /> that shall be populated.
        ///     Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>Thrown if <paramref name="data" /> is <c>null</c>.</para>
        /// </exception>
        public override void GetObjectData(ObjectDataCollection data)
        {
            base.GetObjectData(data);

            data.AddValue(nameof(this.RequestIdentifier), this.RequestIdentifier);
        }
    }
}
