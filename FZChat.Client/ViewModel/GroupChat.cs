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
        private ObservableCollection<ClientUser> groupUsers;
        public ObservableCollection<ClientUser> GroupUsers
        {
            get { return groupUsers; }
            set
            {
                groupUsers = value;
                OnPropertyChanged("GroupUsers");
            }
        }

        public GroupChat(int chatNumber, List<ClientUser> users) : base()
        {
            groupUsers = new ObservableCollection<ClientUser>();
            foreach (ClientUser user in users)
            {
                groupUsers.Add(user);
            }
            this.chatNumber = chatNumber;
        }
    }
}
