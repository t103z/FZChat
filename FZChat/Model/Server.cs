using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FZChat.Model.Utilities;
using System.Net;
using System.Collections.ObjectModel;
using System.Collections;
using System.Net.Sockets;
using System.Threading;

namespace FZChat.Model
{
    public class Server
    {
        private MessageReceiver receiver;
        //private List<OnlineUser> onlineUsers;
        private static Hashtable onlineUsers = new Hashtable();
        private static UserDatabase database = new UserDatabase();
        public event EventHandler<OnlineUserChangedEventArgs> OnlineUserChanged;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived
        {
            add
            {
                receiver.MessageReceived += value;
            }
            remove
            {
                receiver.MessageReceived -= value;
            }
        }

        public event EventHandler<ConnectionLostEventArgs> ConnectionLost
        {
            add
            {
                receiver.ConnectionLost += value;
            }
            remove
            {
                receiver.ConnectionLost -= value;
            }
        }

        public event EventHandler<ClientConnectedEventArgs> ClientConnected
        {
            add
            {
                receiver.ClientConnected += value;
            }
            remove
            {
                receiver.ClientConnected -= value;
            }
        }

        public Server(int port)
        {
            this.receiver = new MessageReceiver(port);
            receiver.MessageReceived += new EventHandler<MessageReceivedEventArgs>(ProcessMessage);
        }

        public void Stop()
        {
            receiver.StopListen();
        }

        private void ProcessMessage(object sender, MessageReceivedEventArgs e)
        {
            string originalString = e.Content;
            //从string获取Message
            Message msg = GenerateMessage(originalString);
            //注册
            if (msg.Type == MessageType.REGISTER)
            {
                //数据库中有此用户，注册失败
                if (database.ContainsUser(msg.Sender))
                {
                    RespondInvalid(e);
                }
                //数据库中无此用户，注册成功
                else
                {
                    RespondOK(e);
                    AddUserToDatabase(database, msg.Content);
                }
            }
            //登录
            else if (msg.Type == MessageType.LOGIN)
            {
                string userName = msg.Sender;
                string password = msg.Content;
                if (CheckIdentity(userName, password))
                {
                    //回应ok
                    RespondOK(e);
                    //将用户加入在线用户表
                    ServerUser fetchedServerUser = database.GetUser(userName);
                    OnlineUser newOnlineUser = new OnlineUser(fetchedServerUser, e.StreamToRemote);
                    onlineUsers.Add(userName, newOnlineUser);
                    //发送用户好友列表
                    SendFriendList(e, userName);
                    Thread.Sleep(100);
                    //发送用户群聊列表
                    SendGroupChatList(e, userName);
                    //发送用户在线好友列表，发送给在线好友JOIN信息
                    SendOnlineFriendList(e, userName);
                    //发送用户离线时收到的信息
                    SendOfflineMessage(e, userName);
                    //触发在线用户改变事件
                    if (OnlineUserChanged != null)
                    {
                        OnlineUserChanged(this, new OnlineUserChangedEventArgs(newOnlineUser.UserName, "online"));
                    }
                }
                else
                {
                    RespondInvalid(e);
                }
            }
            //查询好友
            else if (msg.Type == MessageType.FRIENDSEARCH)
            {
                string[] keyWords = msg.Content.Split(new char[] { '|' });
                string userName = keyWords[0];
                string nickName = keyWords[1];
                string gender = keyWords[2]; //注意此处gender为string以方便查询
                int age = -1;
                if (keyWords[3].Trim().ToLower() != "unlimited")
                {
                    age = int.Parse(keyWords[3].Trim());
                }
                List<ServerUser> usersFound = database.SearchUser(userName, nickName, gender, age);
                if (usersFound.Count == 0)
                {
                    Message newMessage = new Message(MessageType.FRIENDSEARCH, "SERVER", "empty");
                    FowardTo(e, msg.Sender, newMessage);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (ServerUser userFound in usersFound)
                    {
                        string newUserName = userFound.UserName;
                        string newNickName = userFound.NickName;
                        string newGender = userFound.Gender.ToString();
                        string newAge = userFound.Age.ToString();
                        string newEmail = userFound.Email;
                        sb.AppendFormat("{0}|{1}|{2}|{3}|{4}|", newUserName, newNickName,
                            newGender, newAge, newEmail);
                    }
                    string newContent = sb.ToString();
                    Message newMessage = new Message(MessageType.FRIENDSEARCH, "SERVER", newContent);
                    FowardTo(e, msg.Sender, newMessage);
                }
            }
            //私聊，转发给目标用户，写入总聊天记录
            else if (msg.Type == MessageType.PRIV)
            {
                bool IsSuccess = FowardTo(e, msg.Receiver, msg);
                if (IsSuccess)
                {
                    database.AddMessage(msg);
                    RespondOK(e);
                }
                else
                {
                    RespondInvalid(e);
                }
            }
            //群聊，获取群信息，写入总聊天记录，转发给目标用户
            else if (msg.Type == MessageType.GROUP)
            {
                int chatNumber = int.Parse(msg.Receiver);
                ServerChat targetChat = database.GetChat(chatNumber);
                foreach (string userName in targetChat.ChatUserNames)
                {
                    if (userName != msg.Sender)
                    {
                        try
                        {
                            FowardTo(e, userName, msg);
                        }
                        catch
                        {
                            RespondInvalid(e);
                        }
                    }
                }
                database.AddMessage(msg);
                RespondOK(e);
            }
            //用户离线
            else if (msg.Type == MessageType.LEAVE)
            {
                string userName = msg.Sender;
                OnlineUser currentUser = onlineUsers[userName] as OnlineUser;
                //向在线好友发送离线消息
                foreach (string friendName in currentUser.FriendNames)
                {
                    if (onlineUsers.Contains(friendName))
                    {
                        FowardTo(e, friendName, msg);
                    }
                }
                //触发在线用户改变事件
                if (OnlineUserChanged != null)
                {
                    OnlineUserChanged(this, 
                        new OnlineUserChangedEventArgs(onlineUsers[userName] as OnlineUser, "offline"));
                }
                //是否要在此关闭用户连接？
                //将用户移出在线用户列表
                onlineUsers.Remove(userName);
            }
            //好友申请消息
            else if (msg.Type == MessageType.FRIENDREQUEST)
            {
                MakeFriend(msg.Sender, msg.Receiver);
                bool IsSuccess = FowardTo(e, msg.Receiver, msg);
                if (IsSuccess)
                {
                    database.AddMessage(msg);
                    RespondOK(e);
                }
                else
                {
                    RespondInvalid(e);
                }
            }
            //创建群聊
            else if (msg.Type == MessageType.CREATECHAT)
            {
                ServerChat newChat = new ServerChat(msg.Receiver);
                string[] tokens = msg.Content.Split(new char[] { '|' });
                foreach (string token in tokens)
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        newChat.AddUserName(token.Trim());
                    }
                }
                //在数据库中创建群聊
                try
                {
                    database.AddChat(newChat);
                }
                catch
                {
                    RespondInvalid(e);
                }
                //将信息转发给群聊参与者
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0}|", newChat.ChatNumber.ToString());
                sb.AppendFormat("{0}|", newChat.ChatName);
                foreach (var user in newChat.ChatUserNames)
                {
                    sb.AppendFormat("{0}|", user);
                }
                Message msgToClient = new Message(MessageType.CREATECHAT, msg.SendTime, 
                    msg.Sender, sb.ToString());
                bool IsSuccess = true;
                foreach (string userName in newChat.ChatUserNames)
                {
                    IsSuccess = FowardTo(e, userName, msgToClient);
                }
                //回应
                if (IsSuccess)
                {
                    RespondOK(e);
                }
                else
                {
                    RespondOK(e);
                }
            }
            //离开群聊
            else if (msg.Type == MessageType.LEAVECHAT)
            {
                int chatNumber = int.Parse(msg.Content);
                string userName = msg.Sender;
                //在数据库中更新群聊信息
                try
                {
                    database.DeleteChatUser(chatNumber, userName);
                }
                catch
                {
                    RespondInvalid(e);
                }
                //向群聊者推送离开信息
                ServerChat currentChat = database.GetChat(chatNumber);
                bool IsSuccess = true;
                foreach (string name in currentChat.ChatUserNames)
                {
                    if (name != userName)
                    {
                        IsSuccess = FowardTo(e, name, msg);
                    }
                }
                if (IsSuccess)
                {
                    RespondOK(e);
                }
                else
                {
                    RespondOK(e);
                }
            }
        }

        private void SendGroupChatList(MessageReceivedEventArgs e, string userName)
        {
            //格式CREATECHAT|time|SERVER|chatnumber|chatname|username1|...
            List<ServerChat> allChats = database.GetUserGroupChats(userName);
            foreach (var chat in allChats)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0}|{1}|", chat.ChatNumber, chat.ChatName);
                foreach (string name in chat.ChatUserNames)
                {
                    sb.AppendFormat("{0}|", name);
                }
                Message msg = new Message(MessageType.CREATECHAT, "SERVER", sb.ToString());
                FowardTo(e, userName, msg);
                Thread.Sleep(300);
            }
        }

        //使两个用户成为好友
        private void MakeFriend(string sender, string receiver)
        {
            //更新服务端发送请求用户的信息
            OnlineUser currentUser = onlineUsers[sender] as OnlineUser;
            currentUser.FriendNames.Add(receiver);
            //对于接收方，若在线则立刻更新服务端
            if (onlineUsers.Contains(receiver))
            {
                currentUser = onlineUsers[receiver] as OnlineUser;
                currentUser.FriendNames.Add(sender);
            }
            //更新数据库
            database.MakeFriends(sender, receiver);
            //向接收方发送消息在本方法外完成
        }

        //发送在线好友列表
        private void SendOnlineFriendList(MessageReceivedEventArgs e, string userName)
        {
            //格式ONLINELIST|time|SERVER|username1|username2|...
            //注意为空的情况
            StringBuilder sb = new StringBuilder();
            OnlineUser currentUser = onlineUsers[userName] as OnlineUser;
            if (currentUser.FriendNames.Count != 0)
            {
                foreach (string friendName in currentUser.FriendNames)
                {
                    if (onlineUsers.Contains(friendName))
                    {
                        sb.AppendFormat("{0}|", friendName);
                        //发送上线信息，格式为JOIN|time|username
                        Message msgToFriend = new Message(MessageType.JOIN, userName, string.Empty);
                        FowardTo(e, friendName, msgToFriend);
                    }
                }
                string content = sb.ToString();
                Message newMessage = new Message(MessageType.ONLINELIST, "SERVER", content);
                FowardTo(e, userName, newMessage);
            }
        }
        //发送好友信息列表
        private void SendFriendList(MessageReceivedEventArgs e, string userName)
        {
            //格式FRIENDLIST|time|SERVER|username1|nickname1|gender1|age1|email1|...
            //注意为空的情况
            StringBuilder sb = new StringBuilder();
            OnlineUser currentUser = onlineUsers[userName] as OnlineUser;
            string content;
            if (currentUser.FriendNames == null || currentUser.FriendNames.Count == 0)
            {
                return;
            }
            else
            {
                foreach (string name in currentUser.FriendNames)
                {
                    ServerUser currentFriend = database.GetUser(name);
                    sb.AppendFormat("{0}|{1}|{2}|{3}|{4}|", currentFriend.UserName,
                        currentFriend.NickName, currentFriend.Gender.ToString(),
                        currentFriend.Age, currentFriend.Email);
                }
                content = sb.ToString();
                Message newMessage = new Message(MessageType.FRIENDLIST, "SERVER", content);
                FowardTo(e, userName, newMessage);
            }
        }
        //发送离线消息
        private void SendOfflineMessage(MessageReceivedEventArgs e, string userName)
        {
            List<Message> offlineMessages =
                                    database.FetchUserMessage(userName);
            if (offlineMessages.Count != 0)
            {
                foreach (Message offlineMessage in offlineMessages)
                {
                    receiver.SendMessage(offlineMessage, e.StreamToRemote);
                    Thread.Sleep(100);   //防止流写入发生连续写入错误
                }
            }
        }

        private void RespondInvalid(MessageReceivedEventArgs e)
        {
            Message respondMessage = new Message(MessageType.INVALID);
            receiver.SendMessage(respondMessage, e.StreamToRemote);
        }

        private void RespondOK(MessageReceivedEventArgs e)
        {
            Message respondMessage = new Message(MessageType.OK);
            receiver.SendMessage(respondMessage, e.StreamToRemote);
        }

        private bool CheckIdentity(string userName, string password)
        {
            if (database.ContainsUser(userName) == false)
            {
                return false;
            }
            ServerUser currentUser = database.GetUser(userName);
            if (currentUser.Password == password)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddUserToDatabase(UserDatabase database, string content)
        {
            //元数据content = username|password|nickname|gender|age|email|
            //可能的错误：信息中带有“|”
            string[] tokens = content.Split(new char[] { '|' });
            ServerUser newUser = new ServerUser()
            {
                UserName = tokens[0],
                Password = tokens[1],
                NickName = tokens[2],
                Gender = (GenderOption)Enum.Parse(typeof(GenderOption), tokens[3]),
                Age = int.Parse(tokens[4]),
                Email = tokens[5]
            };
            database.AddUser(newUser);
        }

        private Message GenerateMessage(string msgString)
        {
            string[] tokens = msgString.Split(new char[] { '|' });
            DateTime time = DateTime.Parse(tokens[1]);
            if (tokens[0].ToUpper() == "LOGIN")
            {
                Message msg = new Message(MessageType.LOGIN, time, tokens[2], tokens[3]);
                return msg;
            }
            else if (tokens[0].ToUpper() == "REGISTER")
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 2; i < tokens.Length; i++)
                {
                    sb.AppendFormat("{0}|", tokens[i]);
                }
                Message msg = new Message(MessageType.REGISTER, time,
                    tokens[2], sb.ToString());
                return msg;
            }
            else if (tokens[0].ToUpper() == "FRIENDSEARCH")
            {
                string sender = tokens[2];
                StringBuilder sb = new StringBuilder();
                for (int i = 3; i < tokens.Length; i++)
                {
                    sb.AppendFormat("{0}|", tokens[i]);
                }
                Message msg = new Message(MessageType.FRIENDSEARCH, time, sender,
                    sb.ToString());
                return msg;
            }
            else if (tokens[0].ToUpper() == "FRIENDREQUEST")
            {
                string sender = tokens[2];
                string receiver = tokens[3];
                string content = tokens[4];
                Message msg = new Message(MessageType.FRIENDREQUEST, time, sender, receiver, content);
                return msg;
            }
            else if (tokens[0].ToUpper() == "PRIV")
            {
                string sender = tokens[2];
                string receiver = tokens[3];
                string content = tokens[4];
                Message msg = new Message(MessageType.PRIV, time, sender, receiver, content);
                return msg;
            }
            else if (tokens[0].ToUpper() == "GROUP")
            {
                string sender = tokens[2];
                string receiver = tokens[3];
                string content = tokens[4];
                Message msg = new Message(MessageType.GROUP, time, sender, receiver, content);
                return msg;
            }
            //离线格式：LEAVE|time|sender
            else if (tokens[0].ToUpper() == "LEAVE")
            {
                string sender = tokens[2];
                Message msg = new Message(MessageType.LEAVE, time, sender, "");
                return msg;
            }

            else if (tokens[0].ToUpper() == "CREATECHAT")
            {
                string sender = tokens[2];
                string chatName = tokens[3];
                StringBuilder sb = new StringBuilder();
                for (int i = 4; i < tokens.Length; i++)
                {
                    sb.AppendFormat("{0}|", tokens[i]);
                }
                Message msg = new Message(MessageType.CREATECHAT, time, sender, chatName, sb.ToString());
                return msg;
            }

            else if (tokens[0].ToUpper() == "LEAVECHAT")
            {
                string sender = tokens[2];
                string chatNumber = tokens[3];
                Message msg = new Message(MessageType.LEAVECHAT, time, sender, chatNumber);
                return msg;
            }    
            
            //TODO: other circumstences
            else
            {
                Message msg = new Message();
                return msg;
            }
        }

        private bool FowardTo(MessageReceivedEventArgs e, string userName, Message msg)
        {
            //检测用户是否在线
            if (!onlineUsers.Contains(userName))
            {
                //不在线，写入用户离线信息
                database.AddUserMessage(userName, msg);
                return true;
            }
            else
            {
                //在线，直接发送给目标用户
                NetworkStream targetStream = (onlineUsers[userName] as OnlineUser).StreamToRemote;
                return receiver.SendMessage(msg, targetStream);
            }
            
        }

        public ObservableCollection<OnlineUser> GetOnlineUsers()
        {
            ObservableCollection<OnlineUser> obOnlineUsers = new ObservableCollection<OnlineUser>();
            foreach (OnlineUser onlineUser in onlineUsers)
            {
                obOnlineUsers.Add(onlineUser);
            }
            return obOnlineUsers;
        }
    }
}
