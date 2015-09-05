using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Model.Utilities
{
    public class MessageReceivedEventArgs : EventArgs
    {
        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; }
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
    }
}
