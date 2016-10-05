
namespace IndiePortable.Communication.NetClassic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Devices;
    using Devices.ConnectionMessages;

    public partial class TcpConnection
    {


        private readonly ConnectionMessageHandler<ConnectionContentMessage> handlerContent;


        private readonly ConnectionMessageHandler<ConnectionDisconnectRequest> handlerDisconnect;


        private readonly ConnectionMessageHandler<ConnectionMessageKeepAlive> handlerKeepAlive;



        private void HandleContent(ConnectionContentMessage message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.RaiseMessageReceived(message.Content);
        }


        private void HandleDisconnect(ConnectionDisconnectRequest rq)
        {
            if (object.ReferenceEquals(rq, null))
            {
                throw new ArgumentNullException(nameof(rq));
            }

            this.SendConnectionMessage(new ConnectionDisconnectResponse(rq));
        }


        private void HandleKeepAlive(ConnectionMessageKeepAlive message)
        {
            if (object.ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.lastKeepAlive = DateTime.Now;
        }
    }
}
