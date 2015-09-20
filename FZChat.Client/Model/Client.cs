using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FZChat.Model;
using FZChat.Model.Utilities;
using System.Net;
using System.Threading;
using FZChat.Client.Model.Utilities;
using FZChat.Client.ViewModel;
using System.Diagnostics;

namespace FZChat.Client.Model
{
    public enum ResponseType
    {
        OK,
        INVALID,
        ERROR
    }

    public class Client
    {
        private MessageSender sender;
        private Message currentResponse = new Message(MessageType.ERROR, new DateTime());
        private List<string> onlineFriendNames;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived
        {
            add
            {
                sender.MessageReceived += value;
            }
            remove
            {
                sender.MessageReceived -= value;
            }
        }
        public event EventHandler<ConnectionLostEventArgs> ConnectionLost
        {
            add
            {
                sender.ConnectionLost += value;
            }
            remove
            {
                sender.ConnectionLost -= value;
            }
        }

        public event EventHandler<GroupMessageEventArgs> GroupMessageReceived;
        public event EventHandler<PrivateMessageEventArgs> PrivateMessageReceived;
        public event EventHandler<OnlineUserChangedEventArgs> OnlineUserChanged;
        public event EventHandler<FriendInformationEventArgs> FriendInformationReceived;
        public event EventHandler<FriendRequestEventArgs> FriendRequestReceived;
        public event EventHandler<NewChatEventArgs> NewChatCreated;
        public event EventHandler<UserLeaveChatEventArgs> UserLeaveChat;
        
        public Client()
        {
            sender = new MessageSender();
            sender.MessageReceived += ProcessMessage;
            onlineFriendNames = new List<string>();
        }
        private void ProcessMessage(object sender, MessageReceivedEventArgs e)
        {
            string orginalString = e.Content;
            Message msg = GenerateMessage(orginalString);
            switch (msg.Type)
            {
                case MessageType.ONLINELIST:
                    string[] receivedUserNames = msg.Content.Split(new char[] { '|' });
                    foreach (string receivedUserName in receivedUserNames)
                    {
                        UpdateUserOnline(receivedUserName);
                    }
                    break;
                case MessageType.JOIN:
                    UpdateUserOnline(msg.Content);
                    break;
                case MessageType.LEAVE:
                    UpdateUserOffline(msg.Sender);
                    break;
                case MessageType.FRIENDLIST:
                    UpdateFriendInformation(msg.Content);
                    break;
                case MessageType.PRIV:
                    //私聊，触发私聊信息事件
                    if (PrivateMessageReceived != null)
                    {
                        PrivateMessageReceived(this,
                            new PrivateMessageEventArgs(msg.SendTime, msg.Sender
                            , msg.Receiver, msg.Content));
                    }
                    break;
                case MessageType.GROUP:
                    //群聊，触发群聊信息事件
                    if (GroupMessageReceived != null)
                    {
                        GroupMessageReceived(this,
                            new GroupMessageEventArgs(msg.SendTime, msg.Sender
                            , msg.Receiver, msg.Content));
                    }
                    break;
                case MessageType.FRIENDREQUEST:
                    //好友申请，触发事件
                    if (FriendRequestReceived != null)
                    {
                        FriendRequestReceived(this,
                            new FriendRequestEventArgs(msg.Sender, msg.Content));
                    }
                    break;
                case MessageType.FRIENDSEARCH:
                    //好友搜索结果，交由应答处理
                    currentResponse = msg;
                    break;
                case MessageType.OK:
                    currentResponse = msg;
                    Debug.WriteLine("response updated {0}", DateTime.Now);
                    break;
                case MessageType.ERROR:
                    currentResponse = msg;
                    Debug.WriteLine("response updated {0}", DateTime.Now);
                    break;
                case MessageType.INVALID:
                    lock (currentResponse)
                    {
                        currentResponse = msg;
                        Debug.WriteLine("response updated {0}", DateTime.Now);
                    }
                    break;
                case MessageType.CREATECHAT:
                    OnNewChatCreated(msg);
                    break;
                case MessageType.LEAVECHAT:
                    if (UserLeaveChat != null)
                    {
                        UserLeaveChat(this, new UserLeaveChatEventArgs(msg.Sender, msg.Content));
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnNewChatCreated(Message msg)
        {
            string[] tokens = msg.Content.Split(new char[] { '|' });
            string sender = msg.Sender;
            int chatNumber = int.Parse(tokens[0]);
            string chatName = tokens[1];
            List<string> userNames = new List<string>();
            for (int i = 2; i < tokens.Length; i++)
            {
                if (!string.IsNullOrEmpty(tokens[i]))
                {
                    userNames.Add(tokens[i]);
                }
            }
            if (NewChatCreated != null)
            {
                NewChatCreated(this, new NewChatEventArgs(chatNumber, chatName, sender, userNames));
            }
        }

        private void UpdateFriendInformation(string msgString)
        {
            string[] tokens = msgString.Split(new char[] { '|' });
            string userName = string.Empty, nickName = string.Empty, email = string.Empty;
            int age = 18;
            GenderOption gender = GenderOption.Male;
            for (int i = 0; i < tokens.Length; i++)
            {
                if (string.IsNullOrEmpty(tokens[i]))
                {
                    break;
                }
                if (i % 5 == 0)
                {
                    userName = tokens[i];
                }
                else if (i % 5 == 1)
                {
                    nickName = tokens[i];
                }
                else if (i % 5 == 2)
                {
                    gender = (GenderOption)Enum.Parse(typeof(GenderOption), tokens[i]);
                }
                else if (i % 5 == 3)
                {
                    age = int.Parse(tokens[i]);
                }
                else
                {
                    email = tokens[i];
                    ClientUser newUser = new ClientUser(userName, nickName, age, gender, email);
                    if (FriendInformationReceived != null)
                    {
                        FriendInformationReceived(this, new FriendInformationEventArgs(newUser));
                    }
                }
            }
        }

        private void UpdateUserOnline(string receivedUserName)
        {
            string onlineUserName = receivedUserName.Trim(new char[] { '|', ' ' });
            //添加到在线用户列表
            onlineFriendNames.Add(onlineUserName);
            //触发用户状态改变事件
            if (OnlineUserChanged != null)
            {
                OnlineUserChanged(this, new OnlineUserChangedEventArgs(onlineUserName,
                    "online"));
            }
        }

        private void UpdateUserOffline(string sender)
        {
            string onlineUserName = sender.Trim(new char[] { '|', ' ' });
            //从在线用户列表去除
            onlineFriendNames.Remove(onlineUserName);
            //触发用户状态改变事件
            if (OnlineUserChanged != null)
            {
                OnlineUserChanged(this, new OnlineUserChangedEventArgs(onlineUserName,
                    "offline"));
            }
        }

        private Message GenerateMessage(string orginalString)
        {
            string[] tokens = orginalString.Split(new char[] { '|' });
            DateTime time = DateTime.Parse(tokens[1]);
            Message msg;
            StringBuilder sb;
            switch (tokens[0].ToUpper())
            {
                case "OK":
                    msg = new Message(MessageType.OK, time);
                    break;
                case "INVALID":
                    msg = new Message(MessageType.INVALID, time);
                    break;
                case "ERROR":
                    msg = new Message(MessageType.ERROR, time);
                    break;
                case "PRIV":
                    sb = new StringBuilder();
                    for (int i = 4; i < tokens.Length; i++)
                    {
                        sb.AppendFormat("{0} ", tokens[i]);
                    }
                    msg = new Message(MessageType.PRIV, time, tokens[2], tokens[3], sb.ToString());
                    break;
                case "GROUP":
                    sb = new StringBuilder();
                    for (int i = 4; i < tokens.Length; i++)
                    {
                        sb.AppendFormat("{0} ", tokens[i]);
                    }
                    msg = new Message(MessageType.GROUP, time, tokens[2], tokens[3], sb.ToString());
                    break;
                case "ONLINELIST":
                case "FRIENDLIST":
                    sb = new StringBuilder();
                    for (int i = 3; i < tokens.Length; i++)
                    {
                        sb.AppendFormat("{0}|", tokens[i]);
                    }
                    msg = new Message(MessageType.FRIENDLIST, time, "SERVER", sb.ToString());
                    break;
                case "JOIN":
                    msg = new Message(MessageType.JOIN, time, tokens[2], string.Empty);
                    break;
                case "LEAVE":
                    msg = new Message(MessageType.LEAVE, time, tokens[2], string.Empty);
                    break;
                case "FRIENDREQUEST":
                    sb = new StringBuilder();
                    for (int i = 4; i < tokens.Length; i++)
                    {
                        sb.AppendFormat("{0} ", tokens[i]);
                    } 
                    msg = new Message(MessageType.FRIENDREQUEST, time, tokens[2], tokens[3], sb.ToString());
                    break;
                case "FRIENDSEARCH":
                    sb = new StringBuilder();
                    for (int i = 3; i < tokens.Length; i++)
                    {
                        sb.AppendFormat("{0}|", tokens[i]);
                    }
                    msg = new Message(MessageType.FRIENDSEARCH, time, "SERVER", sb.ToString());
                    break;
                case "CREATECHAT":
                    sb = new StringBuilder();
                    for (int i = 3; i < tokens.Length; i++)
                    {
                        sb.AppendFormat("{0}|", tokens[i]);
                    }
                    msg = new Message(MessageType.CREATECHAT, time, tokens[2], sb.ToString());
                    break;
                case "LEAVECHAT":
                    msg = new Message(MessageType.LEAVECHAT, time, tokens[2], tokens[3]);
                    break;
                default:
                    msg = new Message(MessageType.ERROR, time);
                    break;
            }
            return msg;
        }

        public ResponseType Send(Message msg)
        {
            sender.SendMessage(msg);
            Thread.Sleep(500);
            for (int i = 0; i < 10; i++)
            {
                Debug.WriteLine("checked {0}", DateTime.Now);
                if (currentResponse == null)
                {
                    Thread.Sleep(50);
                    continue;
                }
                //等待响应
                if (msg.SendTime.CompareTo(currentResponse.SendTime + new TimeSpan(0,0,0,1,0)) <= 0)
                {
                    Debug.WriteLine("Response handled {0}", DateTime.Now);
                    switch (currentResponse.Type)
                    {
                        case MessageType.INVALID:
                            return ResponseType.INVALID;
                        case MessageType.OK:
                            return ResponseType.OK;
                        case MessageType.ERROR:
                            return ResponseType.ERROR;
                        default:
                            return ResponseType.ERROR;
                    }
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
            Debug.WriteLine("No response {0}", DateTime.Now);
            return ResponseType.ERROR;
        }

        public void SendNoResponse(Message msg)
        {
            sender.SendMessage(msg);
        }

        public List<ClientUser> SearchFriend(Message msg)
        {
            sender.SendMessage(msg);
            Thread.Sleep(500);
            List<ClientUser> usersFound = new List<ClientUser>();
            for (int i = 0; i < 10; i++)
            {
                if (currentResponse == null)
                {
                    Thread.Sleep(50);
                    continue;
                }
                //等待响应
                if (DateTime.Compare(msg.SendTime, currentResponse.SendTime + new TimeSpan(0, 0, 0, 1, 0)) <= 0)
                {
                    string[] tokens = currentResponse.Content.Split(new char[] { '|' });
                    string userName = string.Empty, nickName = string.Empty, email = string.Empty;
                    int age = 18;
                    GenderOption gender = GenderOption.Male;
                    for (int j = 0; j < tokens.Length; j++)
                    {
                        if (tokens[j].Trim() == "empty")
                        {
                            break;
                        }
                        if (string.IsNullOrEmpty(tokens[j]))
                        {
                            break;
                        }
                        if (j % 5 == 0)
                        {
                            userName = tokens[j];
                        }
                        else if (j % 5 == 1)
                        {
                            nickName = tokens[j];
                        }
                        else if (j % 5 == 2)
                        {
                            gender = (GenderOption)Enum.Parse(typeof(GenderOption), tokens[j]);
                        }
                        else if (j % 5 == 3)
                        {
                            age = int.Parse(tokens[j]);
                        }
                        else
                        {
                            email = tokens[j];
                            ClientUser newUser = new ClientUser(userName, nickName, age, gender, email);
                            usersFound.Add(newUser);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(50);
                }
                return usersFound;
            }
            return usersFound;
        }

        public bool Connect(IPAddress ip, int port)
        {
            return sender.Connect(ip, port);
        }
        public void SignOut()
        {
            sender.SignOut();
        }
    }
}
