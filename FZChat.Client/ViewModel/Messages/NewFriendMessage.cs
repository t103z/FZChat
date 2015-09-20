using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Client.ViewModel.Messages
{
    public class NewFriendMessage
    {
        public ClientUser FriendInformation { get; set; }
        public NewFriendMessage(ClientUser user)
        {
            FriendInformation = user;
        }
    }
}
