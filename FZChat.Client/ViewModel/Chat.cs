using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Client.ViewModel
{
    public abstract class Chat : INotifyPropertyChanged
    {
        private ObservableCollection<ChatLog> chatLogs;

        public Chat()
        {
            chatLogs = new ObservableCollection<ChatLog>();
        }

        public ObservableCollection<ChatLog> ChatLogs
        {
            get { return chatLogs; }
            set
            {
                chatLogs = value;
                OnPropertyChanged("ChatLogs");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
