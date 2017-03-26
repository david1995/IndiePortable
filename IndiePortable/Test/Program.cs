using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using IndiePortable.Collections;
using IndiePortable.Communication.Messages;
using IndiePortable.Communication.NetClassic;
using IndiePortable.Communication.Tcp;
using IndiePortable.Formatter;
using TestLib;

namespace Test
{
    static class Program
    {
        static void Main(string[] args)
        {

            using (var signal = new AutoResetEvent(false))
            {
                using (var listener = new TcpConnectionListener())
                {
                    listener.StartListening(new TcpConnectionListenerSettings(8000));

                    listener.ConnectionReceived += async (s, e) =>
                    {
                        e.ReceivedConnection.ConnectionMessageReceived += (_, eargs) => Console.WriteLine($"{eargs.Message.GetType().Name}");
                        e.ReceivedConnection.MessageReceived += (_, eargs) => Console.WriteLine($"{eargs.ReceivedMessage.GetType().Name}");
                        e.ReceivedConnection.Disconnected += (_, eargs) => Console.WriteLine("Received connection disconnected.");
                        e.ReceivedConnection.Activate();
                        await Task.Delay(1000);
                        e.ReceivedConnection.SendMessage(new TextMessage("Hello World"));
                        Console.ReadLine();
                        signal.Set();
                    };

                    signal.WaitOne();
                    listener.StopListening();
                }
            }
        }
    }
}
