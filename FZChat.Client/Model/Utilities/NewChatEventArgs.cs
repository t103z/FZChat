using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Client.Model.Utilities
{
    public class NewChatEventArgs : EventArgs
    {
        public string Invitor { get; set; }
        public int ChatNumber { get; set; }
        public string ChatName { get; set; }
        public List<string> UserNames { get; private set; }
        public NewChatEventArgs(int chatNumber, string name, string invitor, List<string> userNames)
        {
            ChatNumber = chatNumber;
            ChatName = name;
            UserNames = userNames;
            Invitor = invitor;
        }
    }
}
