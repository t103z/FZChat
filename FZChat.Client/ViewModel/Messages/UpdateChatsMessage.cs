using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Client.ViewModel.Messages
{
    public class UpdateChatsMessage
    {
        public ObservableCollection<Chat> ChatsInformation { get; set; }
        public UpdateChatsMessage(ObservableCollection<Chat> chats)
        {
            ChatsInformation = chats;
        }
    }
}
