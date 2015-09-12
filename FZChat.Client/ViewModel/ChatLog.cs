using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Client.ViewModel
{
    public class ChatLog
    {
        public string Content { get; set; }
        public string Sender { get; set; }
        public DateTime Time { get; set; }
        public ChatLog()
        {
        }
        public ChatLog(DateTime time, string sender, string content)
        {
            Time = time;
            Sender = sender;
            Content = content;
        }
    }
}
