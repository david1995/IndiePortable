// <copyright file="CoUdpListener.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndiePortable.Communication
{
    public class CoUdpListener
        : IDisposable
    {
        private bool isDisposed;

        public CoUdpListener()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CoUdpListener"/> class.
        /// </summary>
        ~CoUdpListener()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                }

                this.isDisposed = true;
            }
        }
    }
}
