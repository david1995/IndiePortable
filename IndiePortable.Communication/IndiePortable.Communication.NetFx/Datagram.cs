// <copyright file="Datagram.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.Net;

namespace IndiePortable.Communication
{
    public sealed class Datagram
    {
        public Datagram(int sourcePort, int destinationPort, byte[] body)
        {
            this.SourcePort = sourcePort < IPEndPoint.MinPort || sourcePort > IPEndPoint.MaxPort
                            ? throw new ArgumentOutOfRangeException(nameof(sourcePort))
                            : sourcePort;

            this.DestinationPort = destinationPort < IPEndPoint.MinPort || destinationPort > IPEndPoint.MaxPort
                                 ? throw new ArgumentOutOfRangeException(nameof(destinationPort))
                                 : destinationPort;

            this.Body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public int SourcePort { get; }

        public int DestinationPort { get; }

        public int Length => this.Body.Length;

        public byte[] Body { get; }
    }
}
