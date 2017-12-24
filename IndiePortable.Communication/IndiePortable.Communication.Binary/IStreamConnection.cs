// <copyright file="IStreamConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.IO;

namespace IndiePortable.Communication.Binary
{
    public interface IStreamConnection
        : IDisposable
    {

        Stream PayloadStream { get; }

        void BeginInit();

        void EndInit();
    }
}
