using FZChat.Client.View;
using FZChat.Client.ViewModel.Service;
using FZChat.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FZChat.Client.ViewModel.Messages;
using FZChat.Model;
using System.Windows;
using System.Windows.Media;
using FZChat.Client.Model.Utilities;

namespace FZChat.Client.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TestItem> TestViewItems { get; set; }

        #region Fields
        public event PropertyChangedEventHandler PropertyChanged;
        private Action _closeWindow;
        //数据服务
        private ClientDataService dataService;
        #endregion

        #region Commands
        //命令
        //打开搜索好友界面命令
        public ICommand SearchFriendCommand { get; set; }
        //打开创建群聊界面命令
        public ICommand CreateGroupCommand { get; set; }
        //关闭窗口命令
        public ICommand CloseWindowCommand { get; set; }
        //和好友聊天命令
        public ICommand ChatWithUserCommand { get; set; }
        //发送聊天内容命令
        public ICommand SendMessageCommand { get; set; }
        //选择聊天改变命令
        public ICommand ChatSelectionChangedCommand { get; set; }
        #endregion

        #region Properties

        public string currentUserName { get; set; }

        //选中的聊天
        private Chat selectedChat;
        public Chat SelectedChat
        {
            get { return selectedChat; }
            set
            {
                selectedChat = value;
                OnPropertyChanged("SelectedChat");
            }
        }

        //选中的好友
        private ClientUser selectedUser;
        public ClientUser SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
        }

        //文本框输入内容
        private string textEntered;
        public string TextEntered
        {
            get { return textEntered; }
            set
            {
                textEntered = value;
                OnPropertyChanged("TextEntered");
            }
        }

        //聊天主窗口可见性
        private Visibility chatMainVisibility;
        public Visibility ChatMainVisibility
        {
            get { return chatMainVisibility; }
            set
            {
                chatMainVisibility = value;
                OnPropertyChanged("ChatMainVisibility");
            }
        }
        //聊天列表可见性
        private Visibility chatListVisibility;
        public Visibility ChatListVisibility
        {
            get { return chatListVisibility; }
            set
            {
                chatListVisibility = value;
                OnPropertyChanged("ChatListVisibility");
            }
        }
        //好友列表可见性
        private Visibility friendListVisibility;
        public Visibility FriendListVisibility
        {
            get { return friendListVisibility; }
            set
            {
                friendListVisibility = value;
                OnPropertyChanged("FriendListVisibility");
            }
        }
        //好友主窗口可见性
        private Visibility friendMainVisibility;
        public Visibility FriendMainVisibility
        {
            get { return friendMainVisibility; }
            set
            {
                friendMainVisibility = value;
                OnPropertyChanged("FriendMainVisibility");
            }
        }
        

        //所有私聊和群聊
        public ObservableCollection<Chat> Chats { get; set; }

        //所有好友
        public ObservableCollection<ClientUser> Friends { get; set; }

        #endregion
        
        #region Constructor
        //构造函数
        public MainViewModel(ClientDataService service, Action closeWindow)
        {
            this._closeWindow = closeWindow;
            dataService = service;
            Chats = dataService.GetAllChats();
            Friends = dataService.GetAllFriends();
            dataService.NewChatCreated += OnNewChatCreated;
            currentUserName = dataService.UserName;
            ChatMainVisibility = Visibility.Visible;
            FriendListVisibility = Visibility.Hidden;
            ChatListVisibility = Visibility.Visible;
            FriendMainVisibility = Visibility.Hidden;
            LoadCommands();
        }


        #endregion

        #region EventHandlers
        private void OnNewChatCreated(object sender, NewChatEventArgs e)
        {
            ChatMainVisibility = Visibility.Visible;
            FriendListVisibility = Visibility.Hidden;
            ChatListVisibility = Visibility.Visible;
            FriendMainVisibility = Visibility.Hidden;
        }
        #endregion

        #region CommandMethods

        private void LoadCommands()
        {
            SearchFriendCommand = new CustomCommand(OpenSearchWindow, CanOpenSearchWindow);
            CloseWindowCommand = new CustomCommand(CloseWindow, CanCloseWindow);
            ChatWithUserCommand = new CustomCommand(ChatWithUser, CanChatWithUser);
            SendMessageCommand = new CustomCommand(SendMessage, CanSendMessage);
            CreateGroupCommand = new CustomCommand(OpenCreateGroup, CanOpenCreateGroup);
            ChatSelectionChangedCommand = new CustomCommand(ChangeChatSelection, CanChangeChatSelection);
        }

        private bool CanChangeChatSelection(object obj)
        {
            return true;
        }

        private void ChangeChatSelection(object obj)
        {
            TextEntered = string.Empty;
        }

        private bool CanOpenCreateGroup(object obj)
        {
            return true;
        }

        private void OpenCreateGroup(object obj)
        {
            Window CreateGroupWindow = new CreateGroup(dataService);
            CreateGroupWindow.Show();
        }
        //属于命令SendMessageCommand
        private bool CanSendMessage(object obj)
        {
            if (selectedChat != null && !string.IsNullOrEmpty(textEntered))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //属于命令SendMessageCommand
        private void SendMessage(object obj)
        {
            selectedChat.ChatLogs.Add(new ChatLog(DateTime.Now, dataService.UserName, textEntered));
            if (selectedChat is PrivateChat)
            {
                string receiver = (selectedChat as PrivateChat).RemoteUser.UserName;
                Message msg = new Message(MessageType.PRIV, dataService.UserName, receiver, textEntered);
                dataService.SendChatMessage(msg);
            }
            else
            {
                string groupNumber = (selectedChat as GroupChat).ChatNumber.ToString();
                Message msg = new Message(MessageType.GROUP, dataService.UserName, groupNumber, textEntered);
                dataService.SendChatMessage(msg);
            }
            
            TextEntered = string.Empty;
        }
        //属于命令ChatWithUserCommand
        private bool CanChatWithUser(object obj)
        {
            if (SelectedUser != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //属于命令ChatWithUserCommand
        private void ChatWithUser(object obj)
        {
            ChatListVisibility = Visibility.Visible;
            ChatMainVisibility = Visibility.Visible;
            FriendListVisibility = Visibility.Hidden;
            FriendMainVisibility = Visibility.Hidden;
            //查看有无现有聊天
            bool ExistChat = false;
            if (Chats.Count != 0)
            {
                var searchedChats = from c in Chats
                               where c.Name == selectedUser.NickName
                               select c;
                if (searchedChats.Count() != 0)
                {
                    ExistChat = true;
                }
            }

            //若已存在，则进入聊天
            if (ExistChat)
            {
                SelectedChat = (from c in Chats
                                where c.Name == selectedUser.NickName
                                select c).Single();
            }
            //若不存在，则创建聊天
            else
            {
                PrivateChat newPrivateChat = new PrivateChat(selectedUser);
                Chats.Add(newPrivateChat);
                SelectedChat = newPrivateChat;
                dataService.GetAllChats();
                Utilities.Messenger.Default.Send(new UpdateChatsMessage(Chats));
            }
        }
        //属于命令CloseWindowCommand
        private bool CanCloseWindow(object obj)
        {
            return true;
        }
        //属于命令CloseWindowCommand
        private void CloseWindow(object obj)
        {
            dataService.SignOut();
            this._closeWindow.Invoke();
            Utilities.Messenger.Default.Send<Messages.ShutDownMessage>(new Messages.ShutDownMessage());
        }
        //属于命令SearchFriendCommand
        private bool CanOpenSearchWindow(object obj)
        {
            return true;
        }
        //属于命令SearchFriendCommand
        private void OpenSearchWindow(object obj)
        {
            SearchFriendWindow searchWindow = new SearchFriendWindow(dataService);
        }

        #endregion

        //实现INotifyPropertyChanged接口
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
