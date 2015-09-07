using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Model.Utilities
{
    public class ConnectionLostEventArgs : EventArgs
    {
        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        public ConnectionLostEventArgs()
        {
            content = "Connection Lost";
        }

        public ConnectionLostEventArgs(IPAddress ip, int port)
        {
            content = "Connection Lost " + ip.ToString() + " port " + port.ToString();
        }
    }
}
