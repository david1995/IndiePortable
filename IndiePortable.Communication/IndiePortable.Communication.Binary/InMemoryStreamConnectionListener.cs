using System;
using System.Collections.Generic;
using System.IO;

namespace IndiePortable.Communication.Binary
{
    public class InMemoryStreamConnectionListener
        : IStreamConnectionListener<InMemoryStreamConnection, int>
    {
        private static readonly object OpenedListenerSyncKey = new object();

        private bool isDisposed;

        public event ConnectionRequestHandler<InMemoryStreamConnection, int> ConnectionRequested;

        public InMemoryStreamConnectionListener(int address)
        {
            this.LocalAddress = address;
        }

        ~InMemoryStreamConnectionListener()
        {
            this.Dispose(false);
        }

        private static IDictionary<int, InMemoryStreamConnectionListener> OpenedListeners { get; } =
            new Dictionary<int, InMemoryStreamConnectionListener>();
        public bool IsListening { get; private set; }

        public int LocalAddress { get; }

        public static (MemoryStream write, MemoryStream read) ConnectTo(InMemoryStreamConnectionClient source, int targetPort)
        {
            lock (OpenedListenerSyncKey)
            {
                if (!OpenedListeners.ContainsKey(targetPort))
                {
                    throw new ArgumentException("No listener with the specified port is opened.", nameof(targetPort));
                }

                var outIn = new MemoryStream();
                var inOut = new MemoryStream();
                OpenedListeners[targetPort].OnConnectionRequested(source.Address, inOut, outIn);
                return (outIn, inOut);
            }
        }

        public void StartListening()
        {
            if (this.IsListening)
            {
                throw new InvalidOperationException("Already listening.");
            }

            lock (OpenedListenerSyncKey)
            {
                if (OpenedListeners.ContainsKey(this.LocalAddress))
                {
                    throw new InvalidOperationException("The specified address is already used.");
                }

                OpenedListeners.Add(this.LocalAddress, this);
            }

            this.IsListening = true;
        }

        public void StopListening()
        {
            if (!this.IsListening)
            {
                throw new InvalidOperationException("Not listening.");
            }

            lock (OpenedListenerSyncKey)
            {
                if (!OpenedListeners.Remove(this.LocalAddress))
                {
                    throw new InvalidOperationException("The specified address is not used.");
                }
            }

            this.IsListening = false;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
            }

            if (this.IsListening)
            {
                this.StopListening();
            }

            this.isDisposed = true;
        }

        protected virtual void OnConnectionRequested(int sourceAddress, MemoryStream inputStream, MemoryStream outputStream)
            => this.ConnectionRequested?.Invoke(this, new PendingInMemoryStreamConnection(sourceAddress, inputStream, outputStream));
    }
}
