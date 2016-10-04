// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionContentMessage.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionContentMessage class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices.ConnectionMessages
{
    using System;
    using Formatter;
    using Messages;


    [Serializable]
    public class ConnectionContentMessage
        : ConnectionMessageBase
    {
        /// <summary>
        /// The backing field for the <see cref="Content" /> property.
        /// </summary>
        private readonly MessageBase contentBacking;


        public ConnectionContentMessage(MessageBase content)
        {
            if (object.ReferenceEquals(content, null))
            {
                throw new ArgumentNullException(nameof(content));
            }

            this.contentBacking = content;
        }


        protected ConnectionContentMessage()
        {
        }


        protected ConnectionContentMessage(ObjectDataCollection data)
            : base(data)
        {
            if (!data.TryGetValue(nameof(this.Content), out this.contentBacking))
            {
                throw new ArgumentException();
            }
        }


        public MessageBase Content => this.contentBacking;


        public override void GetObjectData(ObjectDataCollection data)
        {
            base.GetObjectData(data);

            data.AddValue(nameof(this.Content), this.Content);
        }
    }
}
