using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Model.Utilities
{
    public class OnlineUserChangedEventArgs : EventArgs
    {
        public string UserName { get; set; }
        public string Type { get; set; }
        public OnlineUserChangedEventArgs(string userName, string type)
        {
            this.UserName = userName;
            this.Type = type;
        }
    }
}
