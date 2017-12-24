// <copyright file="RtcpHeader.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Rtp.Rtcp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public struct RtcpHeader
    {
        public byte Version { get; }

        public bool Padding { get; }

        public bool Extension { get; }

        public byte CsrcCount { get; }

        public bool Marker { get; }

        public byte PayloadType { get; }

        public short SequenceNumber { get; }

        public int TimeStamp { get; }

        public int SsrcId { get; }

        public IReadOnlyList<int> CsrcIds { get; }

        public short ExtensionHeaderId { get; }

        public short ExtensionHeaderLength { get; }

        public IReadOnlyList<int> ExtensionHeaders { get; }

        private byte FirstByte
            => (byte)(((((this.Version & 0b11) << 6)
            | ((this.Padding ? 1 : 0) << 6))
            | ((this.Extension ? 1 : 0) << 5)) |
            (this.CsrcCount & 0b1111));

        private byte SecondByte
            => (byte)((this.Marker ? 0b1000_0000 : 0)
            | (this.PayloadType & 0b0111_1111));

        public byte[] ToBytes()
        {
            using (var memstr = new MemoryStream())
            {
                memstr.WriteByte(this.FirstByte);
                memstr.WriteByte(this.SecondByte);
                memstr.WriteByte((byte)(this.SequenceNumber >> 8));
                memstr.WriteByte((byte)this.SequenceNumber);

                memstr.Write(BitConverter.GetBytes(this.TimeStamp), 0, sizeof(int));

                memstr.Write(BitConverter.GetBytes(this.SsrcId), 0, sizeof(int));

                for (var n = 0; n < this.CsrcCount; n++)
                {
                    memstr.Write(BitConverter.GetBytes(this.CsrcIds[n]), 0, sizeof(int));
                }

                memstr.WriteByte((byte)(this.ExtensionHeaderId >> 8));
                memstr.WriteByte((byte)this.ExtensionHeaderId);

                memstr.WriteByte((byte)(this.ExtensionHeaderLength >> 8));
                memstr.WriteByte((byte)this.ExtensionHeaderLength);

                for (var n = 0; n < this.ExtensionHeaderLength; n++)
                {
                    memstr.Write(BitConverter.GetBytes(this.ExtensionHeaders[n]), 0, sizeof(int));
                }

                return memstr.ToArray();
            }
        }
    }
}
