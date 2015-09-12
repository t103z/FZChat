using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Client.Model.Utilities
{
    public class FriendRequestEventArgs : EventArgs
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public FriendRequestEventArgs(string sender, string content)
        {
            Sender = sender;
            Content = content;
        }
    }
}
