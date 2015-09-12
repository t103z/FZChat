using FZChat.Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Client.Model.Utilities
{
    public class FriendInformationEventArgs : EventArgs
    {
        public ClientUser User { get; set; }
        public FriendInformationEventArgs(ClientUser user)
        {
            User = user;
        }
    }
}
