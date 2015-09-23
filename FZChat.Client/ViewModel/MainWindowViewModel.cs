using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FZChat.ViewModel.Utilities;
using System.Windows.Input;
using System.IO;
using System.Windows;
using FZChat.Model;
using FZChat.Client.Model;
using FZChat.Client.View;
using System.Security.Cryptography;
using System.Net;
using FZChat.Client.ViewModel.Messages;

namespace FZChat.Client.ViewModel
{
    [Serializable]
    public class SavedLogInOptions
    {
        public  string      UserName { get; set; }
        public  string      Password { get; set; }
        public  bool        RememberPassword { get; set; }
        public  int         PortNumber { get; set; }
        public  string      IpAddress { get; set; }
    }

    [Serializable]
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Action      _closeAction;
        private SavedLogInOptions _loginOptions;
        private Service.ClientDataService dataService;
        private bool isConnected;
        //private bool IsConnected = false;

        private string      userName;
        public  string      UserName
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

        private string      ipAddress;
        public  string      IpAddress
        {
            get
            {
                return ipAddress;
            }
            set
            {
                ipAddress = value;
                OnPropertyChanged("IpAddress");
            }
        }

        private int         portNumber;

        public  int         PortNumber
        {
            get { return portNumber; }
            set {
                portNumber = value;
                OnPropertyChanged("PortNumber");
            }
        }


        private string      password;
        public  string      Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        private bool        rememberPassword;

        public  bool        RememberPassword
        {
            get { return rememberPassword; }
            set
            {
                rememberPassword = value;
                OnPropertyChanged("RememberPassword");
            }
        }

        public ICommand CloseWindowCommand { get; set; }    //关闭窗口命令
        public ICommand LogInCommand { get; set; }          //登录命令
        public ICommand RegisterCommand { get; set; }       //注册命令

        //构造函数
        public MainWindowViewModel(Action closeAction)
        {
            _closeAction = closeAction;
            LoadSettings();
            LoadCommands();
            Utilities.Messenger.Default.Register<Messages.IPChangedMessage>(this, OnIpChanged);
            dataService = new Service.ClientDataService();
            isConnected = false;
            Utilities.Messenger.Default.Register<Messages.ConnectionChangedMessage>(this, OnConnectionChanged);
        }

        private void OnConnectionChanged(ConnectionChangedMessage obj)
        {
            if (obj.state == "online")
            {
                isConnected = true;
            }
        }

        private void OnIpChanged(IPChangedMessage obj)
        {
            ipAddress = obj.IpAddressString;
            portNumber = obj.portNumber;
        }

        private void LoadSettings()
        {
            if (_loginOptions == null)
            {
                if (File.Exists("LogInOptions.xml"))
                {
                    using (var stream = File.OpenRead("LogInOptions.xml"))
                    {
                        var serializer = new XmlSerializer(typeof(SavedLogInOptions));
                        _loginOptions = serializer.Deserialize(stream) as SavedLogInOptions;
                    }
                    UserName = _loginOptions.UserName;
                    Password = _loginOptions.Password;
                    RememberPassword = _loginOptions.RememberPassword;
                    IpAddress = _loginOptions.IpAddress;
                    PortNumber = _loginOptions.PortNumber;
                    if (!RememberPassword)
                    {
                        Password = null;
                    }
                }
                else
                {
                    UserName = null;
                    Password = null;
                    RememberPassword = false;
                    IpAddress = "127.0.0.1";
                    PortNumber = 8500;
                }
            }
        }

        private void LoadCommands()
        {
            CloseWindowCommand = new CustomCommand(CloseWindow, CanCloseWindow);
            LogInCommand = new CustomCommand(LogIn, CanLogIn);
            RegisterCommand = new CustomCommand(OpenRegisterWindow, CanOpenRegisterWindow);
        }

        private bool CanOpenRegisterWindow(object obj)
        {
            return true;
        }

        private void OpenRegisterWindow(object obj)
        {
            Window registerWindow = new Register(dataService, isConnected, ipAddress, portNumber);
            registerWindow.Show();
        }

        private bool CanLogIn(object obj)
        {
            if (UserName != null && Password != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void LogIn(object obj)
        {
            if (!isConnected)
            {
                if (!dataService.Connect(IPAddress.Parse(ipAddress), portNumber))
                {
                    MessageBox.Show("无法连接至服务器");
                    isConnected = false;
                }
                else
                {
                    isConnected = true;
                }
            }
            string passwordEncripted = GetMD5();
            Message logInMessage = new Message(MessageType.LOGIN, DateTime.Now, UserName, passwordEncripted);
            ResponseType logInResponse = dataService.SendLogInMessage(logInMessage);
            if (logInResponse == ResponseType.OK)
            {
                SaveSettings();
                dataService.LoadUserData(userName);
                dataService.UserName = UserName;
                Main mainWindow = new Main(dataService);
                this._closeAction.Invoke();
            }
            else if (logInResponse == ResponseType.INVALID)
            {
                MessageBox.Show("用户名或密码错误");
            }
            else
            {
                MessageBox.Show("网络错误");
            }
        }

        private void SaveSettings()
        {
            SavedLogInOptions loginOptions = new SavedLogInOptions()
            {
                UserName = userName,
                Password = password,
                RememberPassword = rememberPassword,
                IpAddress = ipAddress,
                PortNumber = portNumber
            };
            using (var stream = File.Open("LogInOptions.xml", FileMode.Create))
            {
                var serializer = new XmlSerializer(typeof(SavedLogInOptions));
                serializer.Serialize(stream, loginOptions);
            }
        }

        private bool CanCloseWindow(object obj)
        {
            return true;
        }

        private void CloseWindow(object obj)
        {
            SaveSettings();
            Utilities.Messenger.Default.Send<Messages.ShutDownMessage>(new ShutDownMessage());
            this._closeAction.Invoke();
        }

        private string GetMD5()
        {
            HashAlgorithm alg = HashAlgorithm.Create("MD5");
            byte[] plainData = Encoding.Unicode.GetBytes(password);
            byte[] hashData = alg.ComputeHash(plainData);
            string encripted = string.Empty;
            foreach (byte digit in hashData)
            {
                encripted += digit.ToString("X").PadLeft(2, '0');
            }
            return encripted;
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
