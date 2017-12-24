// <copyright file="SerialConnection.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using IndiePortable.Communication.Core;
using IndiePortable.Communication.Binary;

namespace IndiePortable.Communication
{
    public class SerialConnection
        : StreamConnection
    {
        private SerialPort client;

        public SerialConnection(string portName)
        {
            this.client = portName is null
                        ? throw new ArgumentNullException(nameof(portName))
                        : !SerialPort.GetPortNames().Contains(portName)
                        ? throw new ArgumentException("No port with the specified name could be found.", nameof(portName))
                        : new SerialPort(portName);
        }

        public override Stream PayloadStream => this.client.BaseStream;

        public string PortName => this.client.PortName;

        protected override void InitializeOverride()
        {
            throw new NotImplementedException();
        }

        protected override void ActivateOverride()
        {
            this.client.Open();
        }

        protected override Task DisconnectAsyncOverride()
        {
            this.client.Close();
            return Task.CompletedTask;
        }

        protected override void DisconnectOverride()
        {
            this.client.Close();
        }

        protected override void DisposeUnmanaged()
        {
            if (this.ConnectionState != ConnectionState.Disposing)
            {
                throw new InvalidOperationException("Invalid state.");
            }

            base.DisposeUnmanaged();

            if (this.client.IsOpen)
            {
                this.client.Close();
            }

            this.client.Dispose();
        }
    }
}
