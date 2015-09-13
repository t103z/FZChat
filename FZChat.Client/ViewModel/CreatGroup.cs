using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Client.ViewModel 
{
    class CreatGroup : INotifyPropertyChanged
    {
        private string finding;
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                 PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
               }
            }
        }
}
