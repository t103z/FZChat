using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Model.Utilities
{
    public class OnlineUserChangedEventArgs : EventArgs
    {
        public OnlineUser user { get; set; }
        public string Type { get; set; }
        public OnlineUserChangedEventArgs(OnlineUser user, string type)
        {
            this.user = user;
            this.Type = type;
        }
    }
}
