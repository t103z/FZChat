using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Client.ViewModel
{
    class FindPasswordViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string userName;
        public string UserName
        {
            get
            {
                return UserName;
            }
            set
            {
                userName = value;
                OnPropertyChanged("UserName");
            }
        }

        private string passwordNow;
        public string PasswordNow
        {
            get
            {
                return PasswordNow;
            }
            set
            {
                passwordNow = value;
                OnPropertyChanged("PasswordNow");
            }
        }

        private string newPassword;
        public string NewPassword
        {
            get
            {
                return NewPassword;
            }
            set
            {
                newPassword = value;
                OnPropertyChanged("NewPassword");
            }
        }

        private string conformNewPassword;
        public string ConformNewPassword
        {
            get
            {
                return ConformNewPassword;
            }
            set
            {
                conformNewPassword = value;
                OnPropertyChanged("ConformNewPassword");
            }
        }
    }
}
