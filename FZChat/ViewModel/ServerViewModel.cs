using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FZChat.Model;
using System.Windows.Input;
using FZChat.ViewModel.Utilities;
using FZChat.Model.Utilities;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows;
using System.Net;

namespace FZChat.ViewModel
{
    public class ServerViewModel : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;   //实现INotifyPropertyChanged接口
        private ObservableCollection<string> onlineUsers;       //对应在线用户列表
        private ObservableCollection<string> msgStrings;            //对应消息列表
        private int portNumber;                                     //监听的端口号
        private bool listenStarted;
        private Server server;                                      //Model模块

        #region Properties
        public ICommand StartListenCommand { get; set; }            //开始监听命令

        public ICommand StopListenCommand { get; set; }             //结束监听命令

        public int PortNumber
        {
            get { return portNumber; }
            set
            {
                portNumber = value;
                OnPropertyChanged("PortNumber");
            }
        }

        public bool ListenStarted
        {
            get
            {
                return listenStarted;
            }
            set
            {
                listenStarted = value;
                OnPropertyChanged("ListenStarted");
            }
        }

        public ObservableCollection<string> OnlineUsers
        {
            get
            {
                return onlineUsers;
            }
            set
            {
                onlineUsers = value;
                OnPropertyChanged("OnlineUsers");
            }
        }

        public ObservableCollection<string> MsgStrings
        {
            get
            {
                return msgStrings;
            }
            set
            {
                msgStrings = value;
                OnPropertyChanged("MsgStrings");
            }
        }
        #endregion

        //构造函数
        public ServerViewModel()
        {
            PortNumber = 8500;                                      //设置默认端口号
            listenStarted = false;
            msgStrings = new ObservableCollection<string>();
            onlineUsers = new ObservableCollection<string>();
            LoadCommands();                                         //初始化命令
        }

        //处理OnlineUserChanged事件，更新在线用户列表
        private void ChangeOnlineUser(object sender, OnlineUserChangedEventArgs e)
        {
            if (e.Type.ToLower() == "online")
            {
                //ObservableCollection为线程安全类型，故需要调用UI所在线程对其进行更改
                this.Dispatcher.Invoke(new Action(() => onlineUsers.Add(e.UserName)));
            }
            else if (e.Type.ToLower() == "offline")
            {
                this.Dispatcher.Invoke(new Action(() => onlineUsers.Remove(e.UserName)));
            }
        }

        //处理MessageReceived事件，更新消息列表
        private void UpdateMessage(object sender, MessageReceivedEventArgs e)
        {
            lock (msgStrings)
            {
                try
                {
                    string newMessage = e.Content;
                    this.Dispatcher.Invoke(new Action(() => msgStrings.Add(newMessage)));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                
            }
        }

        private void LoadCommands()
        {
            //命令类型为CustomCommand，定义在FZChat.ViewModel.Utilities
            StartListenCommand = new CustomCommand(StartListen, CanStartListen);
            StopListenCommand = new CustomCommand(StopListen, CanStopListen);
        }

        //实例化命令用
        private bool CanStopListen(object obj)
        {
            if (listenStarted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void StopListen(object obj)
        {
            listenStarted = false;
            server.Stop();
            msgStrings.Add("Listen stopped");
        }

        private bool CanStartListen(object obj)
        {
            if (portNumber > 0 && !listenStarted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void StartListen(object obj)
        {
            Thread serverWorkThread = new Thread(new ThreadStart(StartServer));
            serverWorkThread.IsBackground = true;
            serverWorkThread.Start();
            msgStrings.Add("Starts listening");
            listenStarted = true;
        }

        //在新线程中开启监听（通过Server类，Model模块）
        private void StartServer()
        {
            server = new Server(portNumber);
            server.OnlineUserChanged += ChangeOnlineUser;
            server.MessageReceived += UpdateMessage;
        }

        //实现INotifyPropertyChanged接口用
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
