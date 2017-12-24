// <copyright file="StreamConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.IO;

namespace IndiePortable.Communication.Binary
{
    public abstract class StreamConnection
        : IStreamConnection
    {
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamConnection"/> class.
        /// </summary>
        protected StreamConnection()
        {
        }

        public abstract Stream PayloadStream { get; }

        public bool IsInitialized { get; private set; }

        public bool IsInitializing { get; private set; }

        public void BeginInit()
        {
            if (this.IsInitializing || this.IsInitialized)
            {
                throw new InvalidOperationException();
            }

            this.IsInitializing = true;
        }

        public void EndInit()
        {
            if (!this.IsInitializing)
            {
                throw new InvalidOperationException();
            }

            this.IsInitializing = false;
            this.IsInitialized = true;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.DisposeManaged();
            }

            this.DisposeUnmanaged();

            this.isDisposed = true;
        }

        protected virtual void DisposeManaged()
        {
        }

        protected virtual void DisposeUnmanaged()
        {
        }
    }
}
