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
        public OnlineUser User { get; set; }
        public string Type { get; set; }
        public OnlineUserChangedEventArgs(string userName, string type)
        {
            this.UserName = userName;
            this.Type = type;
        }
        public OnlineUserChangedEventArgs(OnlineUser user, string type)
        {
            this.User = user;
            this.Type = type;
            this.UserName = user.UserName;
        }
    }
}
