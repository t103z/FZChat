using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FZChat.Model.Utilities;
using System.Net;

namespace FZChat.Model
{
    public class Server
    {
        private MessageReceiver receiver;
        public List<OnlineUser> OnlineUsers;
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
            OnlineUsers = new List<OnlineUser>();
            receiver.MessageReceived += new EventHandler<MessageReceivedEventArgs>(ProcessMessage);
        }

        private void ProcessMessage(object sender, MessageReceivedEventArgs e)
        {
            string originalString = e.Content;
            //从string获取Message
            Message msg = GenerateMessage(originalString);

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

            else if (msg.Type == MessageType.LOGIN)
            {
                string userName = msg.Sender;
                string password = msg.Receiver;
                if (CheckIdentity(userName, password))
                {
                    //回应ok
                    RespondOK(e);
                    //发送用户离线时收到的信息
                    SendOfflineMessage(e, userName);
                    //将用户加入在线用户表
                    ServerUser fetchedServerUser = database.GetUser(userName);
                    IPEndPoint endPoint = e.Remote.Client.RemoteEndPoint as IPEndPoint;
                    OnlineUsers.Add(new OnlineUser(fetchedServerUser, endPoint));
                    //触发在线用户改变事件
                    if (OnlineUserChanged != null)
                    {
                        OnlineUserChanged(this, new OnlineUserChangedEventArgs(userName, "online"));
                    }
                }
                else
                {
                    RespondInvalid(e);
                }
            }

            else if (msg.Type == MessageType.FRIEND)
            {
                if (database.ContainsUser(msg.Receiver))
                {
                    RespondOK(e);
                }
                else
                {
                    RespondInvalid(e);
                }
            }

            else if (msg.Type == MessageType.FRIENDREQUEST)
            {
                FowardTo(e, msg.Receiver, msg);
            }

            else if (msg.Type == MessageType.PRIV)
            {
                FowardTo(e, msg.Receiver, msg);
            }

            //else if (msg.Type == MessageType.GROUP)
            //{
            //    FowardTo(e, msg.Receiver, msg);
            //}
        }

        private void SendOfflineMessage(MessageReceivedEventArgs e, string userName)
        {
            List<Message> offlineMessages =
                                    database.FetchUserMessage(userName);
            if (offlineMessages.Count != 0)
            {
                foreach (Message offlineMessage in offlineMessages)
                {
                    receiver.SendMessage(offlineMessage, e.StreamToRemote);
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
                Message msg = new Message(MessageType.LOGIN, time, tokens[2], "", tokens[3]);
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
                    tokens[2], "", sb.ToString());
                return msg;
            }
            else if (tokens[0].ToUpper() == "FRIEND")
            {
                Message msg = new Message(MessageType.FRIEND, time, tokens[2], tokens[3]);
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
            else if (tokens[0].ToUpper() == "CHATROOM")
            {
                string sender = tokens[2];
                StringBuilder sb = new StringBuilder();
                for (int i = 3; i < tokens.Length; i++)
                {
                    sb.AppendFormat("{0}|", tokens[i]);
                }
                string content = sb.ToString();
                Message msg = new Message(MessageType.CHATROOM, time, sender, content);
                return msg;
            }
            
            //TODO: other circumstences
            else
            {
                Message msg = new Message();
                return msg;
            }
        }

        private void FowardTo(MessageReceivedEventArgs e, string userName, Message msg)
        {
            //检测用户是否在线
            var users = from u in OnlineUsers
                        where u.UserName == userName
                        select u;
            if (users.Count() == 0)
            {
                database.AddUserMessage(userName, msg);
            }
            else
            {
                receiver.SendMessage(msg, e.StreamToRemote);
            }
            
        }
    }
}
