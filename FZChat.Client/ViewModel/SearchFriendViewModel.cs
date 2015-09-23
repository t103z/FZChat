using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FZChat.Client.ViewModel.Service;
using System.Windows.Input;
using FZChat.ViewModel.Utilities;
using FZChat.Model;
using System.Windows;
using System.Windows.Controls;
using FZChat.Client.Model;

namespace FZChat.Client.ViewModel
{
    public class SearchFriendViewModel : INotifyPropertyChanged
    {
        private ClientDataService dataService;
        public event PropertyChangedEventHandler PropertyChanged;
        private Action _closeAction;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //输入的姓名
        private string search;
        public string Search
        {
            get
            {
                return search;
            }
            set
            {
                search = value;
                OnPropertyChanged("Search");
            }
        }
        //可选性别
        public ObservableCollection<string> GenderOptions { get; set; }
        //可选年龄
        public ObservableCollection<string> AgeOptions { get; set; }
        //找到的好友
        public ObservableCollection<ClientUser> FriendsFound { get; set; }
        //选中的性别
        public string GenderSelected { get; set; }
        //选中的年龄
        public int AgeSelected { get; set; }

        //命令
        //找人命令
        public ICommand SearchUserCommand { get; set; }
        public ICommand AddFriendCommand { get; set; }

        //构造函数
        public SearchFriendViewModel(ClientDataService service, Action closeAction)
        {
            LoadData();
            LoadCommands();
            dataService = service;
            _closeAction = closeAction;
        }

        private void LoadCommands()
        {
            SearchUserCommand = new CustomCommand(SearchUser, CanSearchUser);
            AddFriendCommand = new CustomCommand(AddFriend, CanAddFriend);
        }

        //属于AddFriendCommand
        private bool CanAddFriend(object obj)
        {
            return true;
        }
        //属于AddFriendCommand
        private void AddFriend(object obj)
        {
            string targetName = (obj as ClientUser).UserName;
            //首先判断是否已经是好友
            var currentUserFriends = dataService.GetAllFriends();
            var searchedUser = from u in currentUserFriends
                               where u.UserName == targetName
                               select u;
            if (searchedUser.Count() != 0)
            {
                MessageBox.Show("你与此用户已是好友");
                return;
            }
            Message msg = new Message(MessageType.FRIENDREQUEST, dataService.UserName, targetName
                , string.Empty);
            ResponseType response = dataService.SendAddFriendMessage(msg);
            if (response == ResponseType.OK)
            {
                ClientUser newFriend = obj as ClientUser;
                Utilities.Messenger.Default.Send(new Messages.NewFriendMessage(newFriend));
                MessageBox.Show("添加好友成功");
            }
            else if (response == ResponseType.INVALID)
            {
                MessageBox.Show("添加好友失败");
            }
            else
            {
                MessageBox.Show("网络错误");
            }
        }

        //属于命令UserSearchCommand
        private bool CanSearchUser(object obj)
        {
            if (search != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //属于命令UserSearchCommand
        private void SearchUser(object obj)
        {
            string sender = dataService.UserName;
            string userName = search;
            string nickName = search;
            string gender = GenderSelected;
            int age = AgeSelected;
            string ageString;
            if (age == -1)
            {
                ageString = "Unlimited";
            }
            else
            {
                ageString = age.ToString();
            }
            string content = userName + "|" + nickName + "|" + gender + "|" + ageString;
            Message msg = new Message(MessageType.FRIENDSEARCH, sender, content);
            var found = dataService.SendFriendSearchMessage(msg);
            FriendsFound.Clear();
            //注意为空情况
            if (found.Count == 0)
            {
                MessageBox.Show("没有找到用户");
            }
            else if (found.Count == 1 && found.First().UserName == dataService.UserName)
            {
                MessageBox.Show("没有找到用户");
            }
            else
            {
                foreach (var user in found)
                {
                    if (user.UserName != dataService.UserName)
                    {
                        FriendsFound.Add(user);
                    }
                }
            }
        }

        private void LoadData()
        {
            AgeOptions = new ObservableCollection<string>();
            GenderOptions = new ObservableCollection<string>();
            FriendsFound = new ObservableCollection<ClientUser>();
            GenderOptions.Add("不限");
            GenderOptions.Add("男");
            GenderOptions.Add("女");
            AgeOptions.Add("不限");
            for (int i = 0; i < 120; i++)
            {
                AgeOptions.Add(i.ToString());
            }
        }
    }
}
