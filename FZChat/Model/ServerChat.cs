using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Model
{
    public class ServerChat
    {
        public string ChatName { get; set; }
        public int ChatNumber { get; set; }
        public List<string> ChatUserNames { get; set; }

        public ServerChat(string chatName, List<string> chatUserNames)
        {
            ChatName = chatName;
            ChatUserNames = chatUserNames;
            DateTime dateNow = DateTime.Now;
            ChatNumber = dateNow.GetHashCode();
        }

        public ServerChat(string chatName, int chatNumber, List<string> chatUserNames)
        {
            ChatName = chatName;
            ChatUserNames = chatUserNames;
            ChatNumber = chatNumber;
        }
    }
}
