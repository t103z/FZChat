using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Client.ViewModel
{
    public class SetWindowViewModel : INotifyPropertyChanged
    {
        private bool newMessageTellMark;
        public bool NewMessageTellMark
        {
            get { return newMessageTellMark; }
            set
            {
                newMessageTellMark = value;
                OnPropertyChanged("NewMessageTellMark");
            }
        }


        private bool autoUpDate;
        public bool AutoUpDate
        {
            get { return autoUpDate; }
            set
            {
                autoUpDate = value;
                OnPropertyChanged("AutoUpDate");
            }
        }

        private bool saveChatMessage;
        public bool SaveChatMessage
        {
            get { return saveChatMessage; }
            set
            {
                saveChatMessage = value;
                OnPropertyChanged("SaveChatMessage");
            }
        }

        private bool savePassword ;
        public bool SavePassword
        {
            get { return savePassword; }
            set
            {
                savePassword = value;
                OnPropertyChanged("SavePassword");
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName)) ;
            }
        }
    }
}
