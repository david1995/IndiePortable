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
        protected ConnectionMessageResponseBase()
        {
        }


        protected ConnectionMessageResponseBase(T request)
        {
            if (object.ReferenceEquals(request, null))
            {
                throw new ArgumentNullException(nameof(request));
            }

            this.requestIdentifierBacking = request.MessageIdentifier;
        }


        protected ConnectionMessageResponseBase(ObjectDataCollection data)
            : base(data)
        {
            if (!data.TryGetValue(nameof(this.RequestIdentifier), out this.requestIdentifierBacking))
            {
                throw new ArgumentException();
            }
        }


        public Guid RequestIdentifier => this.requestIdentifierBacking;


        public override void GetObjectData(ObjectDataCollection data)
        {
            base.GetObjectData(data);

            data.AddValue(nameof(this.RequestIdentifier), this.RequestIdentifier);
        }
    }
}
