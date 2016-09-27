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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
        ///     <para>This constructor is only intended for formatters to create an emtpy <see cref="MessageRequestBase" />.</para>
        /// </remarks>
        protected MessageRequestBase()
        {
        }


        protected MessageRequestBase(ObjectDataCollection data)
            : base(data)
        {
        }


        protected MessageRequestBase(Guid messageIdentifier)
            : base(messageIdentifier)
        {
        }
    }
}
