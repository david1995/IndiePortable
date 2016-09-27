// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageTransciever.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IMessageTransciever interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices
{
    /// <summary>
    /// Represents a logical device that can send and receive <see cref="Messages.MessageBase" /> objects.
    /// </summary>
    /// <seealso cref="IMessageReceiver" />
    /// <seealso cref="IMessageTransmitter" />
    public interface IMessageTransciever
        : IMessageReceiver, IMessageTransmitter
    {
    }
}
