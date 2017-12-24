// <copyright file="CoUdpConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using IndiePortable.AdvancedTasks;
using IndiePortable.Communication.Core;
using IndiePortable.Communication.Binary;

namespace IndiePortable.Communication
{
    /// <summary>
    /// Provides an implementation of a connection-based protocol using UDP.
    /// </summary>
    /// <seealso cref="StreamConnection" />
    public class CoUdpConnection
        : StreamConnection
    {
        /// <summary>
        /// The implemented protocol version.
        /// </summary>
        public const int Version = 0b00_00_00_01;

        /// <summary>
        /// The default initial receive buffer size.
        /// </summary>
        public const int DefaultInitialReceiveBufferSize = 16384;

        private Random randomBodyGenerator = new Random();

        private UdpClient client;

        private long messageId = long.MinValue;

        private MixedQueue<byte> receiveBuffer;

        public CoUdpConnection(IPEndPoint remoteAddress)
            : this(remoteAddress, DefaultInitialReceiveBufferSize)
        {
        }

        public CoUdpConnection(IPEndPoint remoteAddress, int bufferSize)
        {
            this.RemoteAddress = remoteAddress ?? throw new ArgumentNullException(nameof(remoteAddress));

            this.BufferSize = bufferSize <= 0
                            ? throw new ArgumentOutOfRangeException(nameof(bufferSize))
                            : bufferSize;

            this.client = new UdpClient();
            this.receiveBuffer = new MixedQueue<byte>(bufferSize);
            this.PayloadStream = new CudpStream(this);
        }

        public override Stream PayloadStream { get; }

        public IPEndPoint RemoteAddress { get; }

        public int BufferSize { get; }

        protected override void InitializeOverride()
        {
            throw new NotImplementedException();
        }

        protected override void ActivateOverride() => throw new NotImplementedException();

        protected override Task DisconnectAsyncOverride() => throw new NotImplementedException();

        protected override void DisconnectOverride() => throw new NotImplementedException();

        private static byte[] GetLittleEndianBytes(ValueType vt)
        {
            var b = vt is sbyte sbt ? new byte[] { (byte)sbt }
                  : vt is short s ? BitConverter.GetBytes(s)
                  : vt is int i ? BitConverter.GetBytes(i)
                  : vt is long l ? BitConverter.GetBytes(l)
                  : vt is byte bt ? new byte[] { bt }
                  : vt is ushort us ? BitConverter.GetBytes(us)
                  : vt is uint ui ? BitConverter.GetBytes(ui)
                  : vt is ulong ul ? BitConverter.GetBytes(ul)
                  : vt is float f ? BitConverter.GetBytes(f)
                  : vt is double d ? BitConverter.GetBytes(d)
                  : vt is char c ? BitConverter.GetBytes(c)
                  : vt is bool bl ? BitConverter.GetBytes(bl)
                  : throw new InvalidOperationException();

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(b);
            }

            return b;
        }

        private void SendHeartbeat()
        {
            if (!this.IsConnected)
            {
                throw new InvalidOperationException();
            }

            byte[] RandomBody()
            {
                var length = this.randomBodyGenerator.Next(10, 20);
                var ret = new byte[length];
                this.randomBodyGenerator.NextBytes(ret);
                return ret;
            }

            this.SendMessage(Version, CudpPackageType.Heartbeat, RandomBody());
        }

        private void SendConnect()
        {
            if (this.IsConnected)
            {
                throw new InvalidOperationException();
            }

            this.SendMessage(Version, CudpPackageType.Connect, Array.Empty<byte>());
        }

        private void SendAcknowledge(long oldMessageId)
        {
            if (!this.IsConnected)
            {
                throw new InvalidOperationException();
            }

            this.SendMessage(Version, CudpPackageType.Acknowledge, GetLittleEndianBytes(oldMessageId));
        }

        private void SendRefuse(long oldMessageId)
        {
            if (!this.IsConnected)
            {
                throw new InvalidOperationException();
            }

            this.SendMessage(Version, CudpPackageType.Refuse, GetLittleEndianBytes(oldMessageId));
        }

        private void SendContent(byte[] content)
        {
            if (!this.IsConnected)
            {
                throw new InvalidOperationException();
            }

            this.SendMessage(Version, CudpPackageType.Content, content);
        }

        private void SendDisconnect()
        {
            if (!this.IsConnected)
            {
                throw new InvalidOperationException();
            }

            this.SendMessage(Version, CudpPackageType.Disconnect, Array.Empty<byte>());
        }

        private void SendMessage(int version, CudpPackageType type, byte[] body)
        {
            if (this.ConnectionState == ConnectionState.Disposed)
            {
                throw new ObjectDisposedException(nameof(CoUdpConnection));
            }

            lock (this.client)
            {
                var pkg = new CudpPackage(version, unchecked(this.messageId++), type, body);

                using (var memstr = new MemoryStream())
                {
                    memstr.Write(GetLittleEndianBytes(pkg.Version), 0, sizeof(int));
                    memstr.Write(GetLittleEndianBytes((int)pkg.Type), 0, sizeof(int));
                    memstr.Write(GetLittleEndianBytes(pkg.MessageId), 0, sizeof(long));
                    memstr.Write(GetLittleEndianBytes(pkg.Length), 0, sizeof(int));
                    memstr.Write(pkg.Body, 0, pkg.Length);

                    var bts = memstr.ToArray();
                    this.client.Send(bts, bts.Length, this.RemoteAddress);
                }
            }
        }

        private async void ReadStream(ITaskConnection conn)
        {
            if (conn is null)
            {
                throw new ArgumentNullException(nameof(conn));
            }

            while (!conn.MustFinish)
            {
                while (this.client.Available > 0)
                {
                    var dg = await this.client.ReceiveAsync();

                    if (dg.RemoteEndPoint == this.RemoteAddress)
                    {
                        var ep = dg.RemoteEndPoint;
                        var msg = dg.Buffer;

                        Array.ForEach(msg, t => this.receiveBuffer.Enqueue(t));
                    }
                }

                await Task.Delay(4);
            }
        }

        private struct CudpPackage
        {
            public CudpPackage(int version, long messageId, CudpPackageType type, byte[] body)
            {
                this.Version = version < 0
                             ? throw new ArgumentOutOfRangeException(nameof(version))
                             : version;

                this.MessageId = messageId;
                this.Type = type;
                this.Body = body ?? throw new ArgumentNullException(nameof(body));
                this.Length = this.Body.Length;
            }

            public int Version { get; }

            public long MessageId { get; }

            public CudpPackageType Type { get; }

            public int Length { get; }

            public byte[] Body { get; }
        }

        private sealed class CudpStream
            : Stream
        {
            private CoUdpConnection connection;

            public CudpStream(CoUdpConnection connection)
            {
                this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            }

            public override bool CanRead => this.connection.IsConnected && this.connection.IsActivated;

            public override bool CanSeek => false;

            public override bool CanWrite => this.connection.IsConnected && this.connection.IsActivated;

            public override long Length => throw new NotSupportedException("The length of a network stream cannot be determined.");

            public override long Position
            {
                get => throw new NotSupportedException("The position of a pointer inside a network stream cannot be determined.");
                set => throw new NotSupportedException("The position of a pointer inside a network stream cannot be set.");
            }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (buffer is null)
                {
                    throw new ArgumentNullException(nameof(buffer));
                }

                if (offset < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(offset));
                }

                if (count <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(count));
                }

                if (offset + count > buffer.Length)
                {
                    throw new ArgumentException("Offset + count must be smaller than or equals the buffer length.");
                }

                var read = 0;
                while (this.connection.receiveBuffer.Count > 0 && read < count)
                {
                    buffer[offset + read++] = this.connection.receiveBuffer.Dequeue();
                }

                return read;
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

            public override void SetLength(long value) => throw new NotSupportedException("The length of a network stream cannot be set.");

            public override void Write(byte[] buffer, int offset, int count) => this.connection.SendContent(buffer.Skip(offset).Take(count).ToArray());
        }

        private enum CudpPackageType
        {
            Connect = 0b00_00_00_00,

            Acknowledge = 0b00_00_00_01,

            Refuse = 0b00_00_00_10,

            Heartbeat = 0b00_00_00_11,

            Content = 0b00_00_01_00,

            Disconnect = 0b00_00_01_01,
        }

        private class MixedQueue<T>
            : IReadOnlyCollection<T>
        {
            private readonly LinkedList<MixedQueueEntry> entries = new LinkedList<MixedQueueEntry>();

            private readonly int unitCapacity;

            public MixedQueue()
            {
            }

            public MixedQueue(int unitCapacity)
                : this(unitCapacity, Array.Empty<T>())
            {
            }

            public MixedQueue(IEnumerable<T> collection)
                : this(16, collection)
            {
            }

            public MixedQueue(int unitCapacity, IEnumerable<T> collection)
            {
                this.unitCapacity = unitCapacity <= 0
                                  ? throw new ArgumentOutOfRangeException(nameof(unitCapacity))
                                  : unitCapacity;

                foreach (var c in collection ?? throw new ArgumentNullException(nameof(collection)))
                {
                    this.Enqueue(c);
                }
            }

            public int Count { get; private set; }

            public void Enqueue(T item)
            {
                var entry = this.entries.Last.Value;
                if (entry.WritePointer == this.unitCapacity)
                {
                    entry = new MixedQueueEntry(this.unitCapacity);
                    this.entries.AddLast(entry);
                }

                entry.Enqueue(item);
            }

            public T Dequeue()
            {
                var entry = this.entries.First.Value;

                var ret = entry.Dequeue();
                if (entry.ReadPointer == entry.WritePointer && entry.WritePointer == this.unitCapacity)
                {
                    this.entries.Remove(entry);
                }

                return ret;
            }

            public T Peek() => this.entries.First.Value.Peek();

            public IEnumerator<T> GetEnumerator() => this.EnumeratorGenerator().GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            private IEnumerable<T> EnumeratorGenerator()
            {
                foreach (var e in this.entries)
                {
                    foreach (var t in e)
                    {
                        yield return t;
                    }
                }
            }

            private sealed class MixedQueueEntry
                : IEnumerable<T>
            {
                private readonly T[] entries;

                public MixedQueueEntry(int capacity)
                {
                    this.entries = capacity <= 0
                                 ? throw new ArgumentOutOfRangeException(nameof(capacity))
                                 : new T[capacity];
                }

                public int ReadPointer { get; private set; } = 0;

                public int WritePointer { get; private set; } = 0;

                public void Enqueue(T item)
                {
                    if (this.WritePointer == this.entries.Length)
                    {
                        throw new InvalidOperationException("Entry is full.");
                    }

                    this.entries[this.WritePointer++] = item;
                }

                public T Dequeue()
                {
                    if (this.ReadPointer == this.WritePointer)
                    {
                        throw new InvalidOperationException("Entry is empty.");
                    }

                    return this.entries[this.ReadPointer++];
                }

                public T Peek()
                {
                    if (this.ReadPointer == this.WritePointer)
                    {
                        throw new InvalidOperationException("Entry is empty.");
                    }

                    return this.entries[this.ReadPointer];
                }

                public IEnumerator<T> GetEnumerator() => this.EnumerableGenerator().GetEnumerator();

                IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

                private IEnumerable<T> EnumerableGenerator()
                {
                    foreach (var t in this.entries.Skip(this.ReadPointer).Take(this.WritePointer - this.ReadPointer))
                    {
                        yield return t;
                    }
                }
            }
        }
    }
}
