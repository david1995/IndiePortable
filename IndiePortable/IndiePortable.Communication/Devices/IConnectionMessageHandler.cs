// ----------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionMessageHandler.cs" company="David Eiwen">
// Copyright © 2016 by David Eiwen
// </copyright>
// <author>David Eiwen</author>
// <summary>
// This file contains the IConnectionMessageHandler interface.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------------------------

namespace IndiePortable.Communication.Devices
{
    using System;
    using ConnectionMessages;


    public interface IConnectionMessageHandler
    {

        Type MessageType { get; }


        void HandleMessage(ConnectionMessageBase message);
    }
}
