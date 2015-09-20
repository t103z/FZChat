using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Client.ViewModel
{
    public class GroupChat : Chat, INotifyPropertyChanged
    {
        private int chatNumber;
        public  int ChatNumber
        {
            get { return chatNumber; }
            set
            {
                chatNumber = value;
                OnPropertyChanged("ChatNumber");
            }
        }
        private ObservableCollection<string> groupUsers;
        public ObservableCollection<string> GroupUsers
        {
            get { return groupUsers; }
            set
            {
                groupUsers = value;
                OnPropertyChanged("GroupUsers");
            }
        }

        public GroupChat(int chatNumber, string chatName, List<string> users) : base(chatName)
        {
            groupUsers = new ObservableCollection<string>();
            foreach (string user in users)
            {
                groupUsers.Add(user);
            }
            this.chatNumber = chatNumber;
            this.Name = "[群聊]" + chatName;
        }
    }
}
