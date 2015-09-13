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

namespace FZChat.Client.ViewModel
{
    [Serializable]
    public class SavedLogInOptions
    {
        public  string      UserName { get; set; }
        public  string      Password { get; set; }
        public  bool        RememberPassword { get; set; }
        public  bool        AutoLogIn { get; set; }
    }

    [Serializable]
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Action      _closeAction;
        private SavedLogInOptions _loginOptions;
        private Service.ClientDataService dataService;
        private bool IsConnected = false;

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

        private bool        autoLogIn;

        public  bool        AutoLogIn
        {
            get { return autoLogIn; }
            set
            {
                autoLogIn = value;
                OnPropertyChanged("AutoLogIn");
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
            dataService = new Service.ClientDataService();
            if (!dataService.Connect())
            {
                MessageBox.Show("无法连接至服务器");
            }
            else
            {
                IsConnected = true;
            }
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
                    AutoLogIn = _loginOptions.AutoLogIn;
                    RememberPassword = _loginOptions.RememberPassword;
                    if (!RememberPassword)
                    {
                        Password = null;
                    }
                }
                else
                {
                    UserName = null;
                    Password = null;
                    AutoLogIn = false;
                    RememberPassword = false;
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
            Window registerWindow = new Register(dataService);
            registerWindow.Show();
        }

        private bool CanLogIn(object obj)
        {
            if (UserName != null && Password != null && IsConnected)
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
            string passwordEncripted = GetMD5();
            Message logInMessage = new Message(MessageType.LOGIN, UserName, passwordEncripted);
            ResponseType logInResponse = dataService.SendLogInMessage(logInMessage);
            if (logInResponse == ResponseType.OK)
            {

            }
            else
            {
                MessageBox.Show("用户名或密码错误");
            }
        }

        private void SaveSettings()
        {
            SavedLogInOptions loginOptions = new SavedLogInOptions()
            {
                UserName = userName,
                Password = password,
                AutoLogIn = autoLogIn,
                RememberPassword = rememberPassword
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
            this._closeAction.Invoke();
        }

        private string GetMD5()
        {
            HashAlgorithm alg = HashAlgorithm.Create("MD5");
            byte[] plainData = Encoding.Unicode.GetBytes(password);
            byte[] hashData = alg.ComputeHash(plainData);
            return Encoding.Unicode.GetString(hashData);
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
