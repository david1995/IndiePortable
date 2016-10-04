// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionMessageRequestBase.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the ConnectionMessageRequestBase class.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices.ConnectionMessages
{
    using Formatter;


    [Serializable]
    public abstract class ConnectionMessageRequestBase
        : ConnectionMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMessageRequestBase" /> class.
        /// </summary>
        protected ConnectionMessageRequestBase()
        {
        }


        protected ConnectionMessageRequestBase(ObjectDataCollection data)
            : base(data)
        {
        }
    }
}
