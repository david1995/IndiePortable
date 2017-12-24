using System;
using System.IO;

namespace IndiePortable.Communication.Binary
{
    public class InMemoryStreamConnection
        : StreamConnection
    {
        private readonly InOutMemoryStream stream;
        
        public InMemoryStreamConnection(MemoryStream input, MemoryStream output)
        {
            this.stream = new InOutMemoryStream(
                input ?? throw new ArgumentNullException(nameof(input)),
                output ?? throw new ArgumentNullException(nameof(output)));
        }

        public override Stream PayloadStream => this.stream;

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();
            this.stream.Dispose();
        }

        private class InOutMemoryStream
            : Stream
        {
            private readonly MemoryStream input;
            private readonly MemoryStream output;

            public InOutMemoryStream(MemoryStream input, MemoryStream output)
            {
                this.input = input ?? throw new ArgumentNullException(nameof(input));
                this.output = output ?? throw new ArgumentNullException(nameof(output));
            }

            public override void Flush()
            {
                this.output.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
                => this.input.Read(buffer, offset, count);

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                this.output.Write(buffer, offset, count);
            }

            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => true;

            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }
        }
    }
}
