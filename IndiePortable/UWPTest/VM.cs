using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IndiePortable.AdvancedTasks;
using IndiePortable.Communication.Devices;
using IndiePortable.Communication.Tcp;
using IndiePortable.Communication.UniversalWindows;
using TestLib;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.UI.Core;

namespace UWPTest
{



    public class VM
        : INotifyPropertyChanged
    {


        private IConnection<IPPortAddressInfo> connection;
        private string remoteHostName;
        private ushort remotePort;
        private string output;
        private bool isInactive = true;

        public VM(CoreDispatcher dispatcher)
        {
            this.StartCommand = new RelayCommand(o => this.Start());
            this.Dispatcher = dispatcher;
        }


        public CoreDispatcher Dispatcher { get; }


        public event PropertyChangedEventHandler PropertyChanged;


        public ICommand StartCommand { get; private set; }


        public string RemoteHostName
        {
            get
            {
                return this.remoteHostName;
            }

            set
            {
                this.remoteHostName = value ?? string.Empty;
                this.RaisePropertyChanged();
            }
        }


        public ushort RemotePort
        {
            get
            {
                return this.remotePort;
            }

            set
            {
                this.remotePort = value;
                this.RaisePropertyChanged();
            }
        }


        public bool IsInactive
        {
            get
            {
                return this.isInactive;
            }

            private set
            {
                this.isInactive = value;
                this.RaisePropertyChanged();
            }
        }


        public string Output
        {
            get
            {
                return this.output;
            }

            private set
            {
                this.output = value;
                this.RaisePropertyChanged();
            }
        }


        public async void Start()
        {
            var socket = new StreamSocket();
            var host = new HostName(this.RemoteHostName);
            await socket.ConnectAsync(host, this.remotePort.ToString());
            var ipport = new IPPortAddressInfo(this.RemoteHostName.Split('.').Select(s => byte.Parse(s)).ToArray(), this.RemotePort);

            this.connection = new TcpConnection(socket, ipport);
            this.connection.MessageReceived += async (s, e) => await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.Output = $"{this.output}{e.ReceivedMessage.GetType()}\r\n");
            this.connection.Disconnected += async (s, e) => await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.Output = $"{this.output}Disconnected.\r\n");
            this.connection.Disconnected += async (s, e) => await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.IsInactive = true);
            this.connection.MessageReceived += async (s, e) =>
            {
                if (e.ReceivedMessage is TextMessage)
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    this.Output = $"{this.Output} TextMessage: {(e.ReceivedMessage as TextMessage).Message}\r\n");
                }
            };
            this.connection.Activate();
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.IsInactive = false);
        }





        protected void RaisePropertyChanged([CallerMemberName]string property = null)
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }


    public class RelayCommand
        : ICommand
    {

        private Action<object> method;

        public RelayCommand(Action<object> method)
        {
            this.method = method;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
            => this.method(parameter);
    }
}
