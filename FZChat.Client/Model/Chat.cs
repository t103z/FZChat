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
        public ObservableCollection<ChatLog> ChatLogs
        {
            get { return chatLogs; }
            set
            {
                chatLogs = value;
                OnPropertyChanged("ChatLogs");
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private string summary;
        public string Summary
        {
            get { return summary; }
            set
            {
                summary = value;
                OnPropertyChanged("Summary");
            }
        }

        public Chat(string chatName)
        {
            chatLogs = new ObservableCollection<ChatLog>();
            name = chatName;
            if (chatLogs.Count != 0)
            {
                summary = chatLogs.Last().Content;
            }
        }

        public void AddChatLog(ChatLog log)
        {
            chatLogs.Add(log);
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
