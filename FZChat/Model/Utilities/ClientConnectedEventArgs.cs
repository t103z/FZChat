using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Model.Utilities
{
    public class ClientConnectedEventArgs : EventArgs
    {
        public TcpClient RemoteClient { get; set; }

        public ClientConnectedEventArgs(TcpClient client)
        {
            RemoteClient = client;
        }
    }
}
