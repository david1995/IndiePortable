// <copyright file="RtspServer.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using IndiePortable.Communication.Binary;

namespace IndiePortable.Communication.Rtsp
{
    public class RtspServer
        : IStreamConnectionListener
    {
        private bool isDisposed;

        public RtspServer(IStreamConnectionListener baseListener)
        {
            this.BaseListener = baseListener ?? throw new ArgumentNullException(nameof(baseListener));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RtspServer"/> class.
        /// </summary>
        ~RtspServer()
        {
            this.Dispose(false);
        }

        public event StreamConnectionAcceptedEventHandler ConnectionRequested;

        public bool IsListening { get; private set; }

        public IStreamConnectionListener BaseListener { get; }

        public void StartListening()
        {
            if (this.IsListening)
            {
                throw new InvalidOperationException();
            }

            this.IsListening = true;
            throw new NotImplementedException();
        }

        public void StopListening()
        {
            if (!this.IsListening)
            {
                throw new InvalidOperationException();
            }

            this.IsListening = false;
            throw new NotImplementedException();
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

            this.BaseListener.Dispose();
            this.isDisposed = true;
        }
    }
}
