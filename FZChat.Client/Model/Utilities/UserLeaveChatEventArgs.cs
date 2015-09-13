using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Client.Model.Utilities
{
    public class UserLeaveChatEventArgs : EventArgs
    {
        public string UserName { get; set; }
        public string ChatNumber { get; set; }
        public UserLeaveChatEventArgs(string userName, string chatNumber)
        {
            UserName = userName;
            ChatNumber = chatNumber;
        }
    }
}
