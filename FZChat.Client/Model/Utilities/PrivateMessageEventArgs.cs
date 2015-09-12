using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FZChat.Model;

namespace FZChat.Client.Model.Utilities
{
    public class PrivateMessageEventArgs : EventArgs
    {
        public Message msg { get; set; }
        public PrivateMessageEventArgs(DateTime time, string sender, string receiver, string content)
        {
            msg = new Message(MessageType.PRIV, time, sender, receiver, content);
        }
    }
}
