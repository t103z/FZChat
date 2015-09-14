using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FZChat.Model;
using System.ComponentModel;

namespace FZChat.Client.ViewModel
{
    public enum OnlineStatus
    {
        OFFLINE,
        ONLINE
    }
    public class ClientUser : IUser, INotifyPropertyChanged
    {
        private int             age;
        private string          email;
        private GenderOption    gender;
        private int             head;
        private string          userName;
        private string          nickName;
        private OnlineStatus    status;

        public  int             Age
        {
            get
            {
                return age;
            }

            set
            {
                age = value;
                OnPropertyChanged("Age");
            }
        }

        public  string          Email
        {
            get
            {
                return email;
            }

            set
            {
                email = value;
                OnPropertyChanged("Email");
            }
        }

        public  GenderOption    Gender
        {
            get
            {
                return gender;
            }

            set
            {
                gender = value;
                OnPropertyChanged("Gender");
            }
        }

        public  int             Head
        {
            get
            {
                return head;
            }

            set
            {
                head = value;
                OnPropertyChanged("Head");
            }
        }

        public  string          NickName
        {
            get
            {
                return nickName;
            }

            set
            {
                nickName = value;
                OnPropertyChanged("NickName");
            }
        }

        public  string          UserName
        {
            get
            {
                return userName;
            }

            set
            {
                userName = value;
                OnPropertyChanged("UserName");
            }
        }

        public  OnlineStatus    Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        public  event PropertyChangedEventHandler PropertyChanged;

        public ClientUser(string userName, string nickName, int age, GenderOption gender,
            string email, OnlineStatus status)
        {
            this.userName = userName;
            this.nickName = nickName;
            this.age = age;
            this.gender = gender;
            this.email = email;
            this.status = status;
        }

        public ClientUser(string userName, string nickName, int age, GenderOption gender,
            string email)
        {
            this.userName = userName;
            this.nickName = nickName;
            this.age = age;
            this.gender = gender;
            this.email = email;
            this.status = OnlineStatus.OFFLINE;
        }

        public void LogIn()
        {
            this.status = OnlineStatus.ONLINE;
        }

        public void LogOff()
        {
            this.status = OnlineStatus.OFFLINE;
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
