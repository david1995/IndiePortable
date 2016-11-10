using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IndiePortable.AdvancedTasks;
using IndiePortable.Communication.Messages;
using IndiePortable.Communication.NetClassic;
using IndiePortable.Communication.Tcp;
using IndiePortable.Formatter;

namespace TestCommNetFramework
{
    class Program
    {
        [IndiePortable.Formatter.Serializable]
        private class TestMessage
            : MessageBase
        {
            private string textBacking;

            protected TestMessage()
            {
            }

            public TestMessage(string text)
            {
                this.textBacking = text;
            }

            protected TestMessage(ObjectDataCollection data) : base(data)
            {
                if (!data.TryGetValue(nameof(this.Text), out this.textBacking))
                {
                    throw new ArgumentException();
                }
            }

            protected TestMessage(Guid identifier) : base(identifier)
            {
            }

            public string Text => this.textBacking;

            public override void GetObjectData(ObjectDataCollection data)
            {
                base.GetObjectData(data);
                data.AddValue(nameof(this.Text), this.Text);
            }
        }

        static void Main(string[] args)
        {
            var task1 = new StateTask(Task1Method);
            Thread.Sleep(10000);
            var task2 = new StateTask(Task2Method);

            while (Console.ReadKey().Key != ConsoleKey.Enter)
            {
                Thread.Sleep(5);
            }

            task1.Stop();
            task2.Stop();
        }


        private static async void Task1Method(ITaskConnection conn)
        {
            var listener = new TcpConnectionListener();
            var connections = new List<TcpConnection>();
            listener.ConnectionReceived += (s, e) =>
            {
                e.ReceivedConnection.MessageReceived += (_, ea) => Console.WriteLine($"[Server] Received {ea.GetType()}");
                connections.Add(e.ReceivedConnection);
                e.ReceivedConnection.Activate();
            };

            listener.StartListening(new TcpConnectionListenerSettings(1000));

            while (!conn.MustFinish)
            {
                await Task.Delay(5);
            }

            listener.StopListening();
            conn.Return();
        }


        private static async void Task2Method(ITaskConnection conn)
        {
            var cl = new TcpClient();
            await cl.ConnectAsync("localhost", 1000);
            var ep = (IPEndPoint)cl.Client.RemoteEndPoint;
            using (var connection = new TcpConnection(cl, new IPPortAddressInfo(ep.Address.GetAddressBytes(), (ushort)ep.Port)))
            {
                connection.Activate();
                while (!conn.MustFinish)
                {
                    await connection.SendMessageAsync(new TestMessage("Hallo welt"));
                    Console.WriteLine("[Client] Sent message");
                    await Task.Delay(1000);
                }

            }
            conn.Return();
        }
    }
}
