using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Model.Utilities
{
    public class MessageReceivedEventArgs : EventArgs
    {
        private string content;
        private NetworkStream streamToRemote;
        private TcpClient remote;

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        public NetworkStream StreamToRemote
        {
            get { return streamToRemote; }
            set { streamToRemote = value; }
        }

        public TcpClient Remote
        {
            get { return remote; }
            set { remote = value; }
        }

        public MessageReceivedEventArgs()
        {
            content = "EMPTY|emptymessage|";
        }

        public MessageReceivedEventArgs(Message msg)
        {
            content = msg.ToString();
        }

        public MessageReceivedEventArgs(string msg)
        {
            content = msg;
        }

        public MessageReceivedEventArgs(string msg, NetworkStream stream)
        {
            content = msg;
            streamToRemote = stream;
        }
    }
}
