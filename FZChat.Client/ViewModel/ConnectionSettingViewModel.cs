using FZChat.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FZChat.Client.ViewModel
{
    [Serializable]
    public class ConnectionSettingViewModel : INotifyPropertyChanged
    {
        private Action _closeAction;

        private string ipAddress;

        public string IpAddress
        {
            get { return ipAddress; }
            set
            {
                ipAddress = value;
                OnPropertyChanged("IpAddress");
            }
        }

        private int portNumber;

        public int PortNumber
        {
            get { return portNumber; }
            set
            {
                portNumber = value;
                OnPropertyChanged("PortNumber");
            }
        }

        public ICommand EnterCommand { get; set; }

        public ConnectionSettingViewModel(Action closeAction)
        {
            this._closeAction = closeAction;
            EnterCommand = new CustomCommand(MakeChanges, CanMakeChanges);
        }

        private bool CanMakeChanges(object obj)
        {
            if (!string.IsNullOrEmpty(ipAddress))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void MakeChanges(object obj)
        {
            Utilities.Messenger.Default.Send(new Messages.IPChangedMessage(IpAddress, portNumber));
            this._closeAction.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string parameter)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(parameter));
            }
        }
    }
}
