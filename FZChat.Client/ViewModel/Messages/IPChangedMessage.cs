using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Client.ViewModel.Messages
{
    public class IPChangedMessage
    {
        public string IpAddressString { get; set; }
        public int portNumber { get; set; }
        public IPChangedMessage(string ip, int port)
        {
            IpAddressString = ip;
            portNumber = port;
        }
    }
}
