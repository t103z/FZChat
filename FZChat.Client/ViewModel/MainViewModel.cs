using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Client.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<TestItem> TestViewItems { get; set; }

        //选中的群聊
        private Chat selectedChat;
        public  Chat SelectedChat
        {
            get { return selectedChat; }
            set
            {
                selectedChat = value;
                OnPropertyChanged("SelectedChat");
            }
        }
        
        public ObservableCollection<Chat> Chats { get; set; }

        private string finding;/*搜索*/
        public string Finding
        {
            get
            {
                return finding;
            }
            set
            {
                finding = value;
                OnPropertyChanged("Finding");
            }
        }

        private string remarkText;/*备注*/
        public string RemarkText
        {
            get
            {
                return remarkText;
            }
            set
            {
                remarkText = value;
                OnPropertyChanged("RemarkText");
            }
        }

        private string fzNumber; /*风竹号*/
        public string Fznumber
        {
            get
            {
                return fzNumber;
            }
            set
            {
                fzNumber = value;
                OnPropertyChanged("FzNumber");
            }
        }

        private string messageUserName;/*好友信息用户名*/
        public string MessageUserName
        {
            get
            {
                return messageUserName;
            }
            set
            {
                messageUserName = value;
                OnPropertyChanged("MessageUserName");
            }
        }

        private string district;/*地区*/
        public string District
        {
            get
            {
                return district;
            }
            set
            {
                district = value;
                OnPropertyChanged("District");
            }
        }

        private string chatGroupName;/*群聊名*/
        public string ChatGroupName
        {
            get
            {
                return chatGroupName;
            }
            set
            {
                chatGroupName = value;
                OnPropertyChanged("ChatGroupName");
            }
        }



        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
