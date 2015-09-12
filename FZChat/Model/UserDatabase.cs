using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FZChatServer.Orm;
using System.Data.Linq;

namespace FZChat.Model
{
    public class UserDatabase
    {
        private DataClassesDataContext dataContext = new DataClassesDataContext();
        //添加用户
        public void AddUser(ServerUser user)
        {
            DatabaseUser newUser = new DatabaseUser()
            {
                UserName = user.UserName,
                NickName = user.NickName,
                Gender = user.Gender.ToString(),
                Email = user.Email,
                Age = user.Age,
                Password = user.Password
            };
            foreach (string username in user.FriendNames)
            {
                UserFriend newFriend = new UserFriend() { UserName = username };
                newUser.UserFriend.Add(newFriend);
            }
            dataContext.DatabaseUser.InsertOnSubmit(newUser);
            dataContext.SubmitChanges();
        }
        //删除用户
        public void RemoveUser(string userName)
        {
            var selectedUser = (from u in dataContext.DatabaseUser
                                where u.UserName == userName
                                select u).Single();
            //删除用户好友记录
            if (selectedUser.UserFriend.Count() > 0)
            {
                var selectedUserFriend = from f in selectedUser.UserFriend
                                         select f;
                foreach (UserFriend userFriend in selectedUserFriend)
                {
                    dataContext.UserFriend.DeleteOnSubmit(userFriend);
                }
            }
            //删除用户离线数据记录
            if (selectedUser.DatabaseUserMessage.Count() > 0)
            {
                var selectedMessage = from m in selectedUser.DatabaseUserMessage
                                      select m;
                foreach (DatabaseUserMessage msg in selectedMessage)
                {
                    dataContext.DatabaseUserMessage.DeleteOnSubmit(msg);
                }
            }
            //删除用户群聊记录
            if (selectedUser.DatabaseUserChat.Count() > 0)
            {
                var selectedChat = from c in selectedUser.DatabaseUserChat
                                   select c;
                //从群聊中删除用户
                foreach (DatabaseUserChat chat in selectedChat)
                {
                    int chatNumber = chat.Number;
                    var groupChat = (from g in dataContext.DatabaseGroupChat
                                     where g.ChatNumber == chatNumber
                                     select g).Single();
                    var groupChatUser = (from u in groupChat.DatabaseGroupChatUser
                                         where u.UserName == userName
                                         select u).Single();
                    dataContext.DatabaseGroupChatUser.DeleteOnSubmit(groupChatUser);
                    dataContext.DatabaseUserChat.DeleteOnSubmit(chat);
                }
                
            }
            dataContext.DatabaseUser.DeleteOnSubmit(selectedUser);
            dataContext.SubmitChanges();
        }
        //更新用户信息
        public void UpdateUser(string userName, ServerUser newUser)
        {
            this.RemoveUser(userName);
            this.AddUser(newUser);
        }
        public bool ContainsUser(string userName)
        {
            var selectedUsers = from u in dataContext.DatabaseUser
                                where u.UserName == userName
                                select u;
            if (selectedUsers.Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //获取用户信息
        public ServerUser GetUser(string userName)
        {
            var selectedUser = (from u in dataContext.DatabaseUser
                                where u.UserName == userName
                                select u).Single();
            ServerUser newServerUser = new ServerUser()
            {
                UserName = selectedUser.UserName,
                NickName = selectedUser.NickName,
                Age = selectedUser.Age,
                Email = selectedUser.Email,
                Gender = (GenderOption)Enum.Parse(typeof(GenderOption), selectedUser.Gender),
                Password = selectedUser.Password,
                FriendNames = new List<string>()
            };
            foreach (UserFriend userFriend in selectedUser.UserFriend)
            {
                newServerUser.FriendNames.Add(userFriend.UserName);
            }
            return newServerUser;
        }
        public void AddUserMessage(string userName, Message msg)
        {
            DatabaseUser targetUser = (from u in dataContext.DatabaseUser
                                       where u.UserName == userName
                                       select u).Single();
            DatabaseUserMessage newUserMessage = new DatabaseUserMessage()
            {
                Time = msg.SendTime,
                Sender = msg.Sender,
                Receiver = msg.Receiver,
                Content = msg.Content,
                Type = msg.Type.ToString()
            };
            targetUser.DatabaseUserMessage.Add(newUserMessage);
            dataContext.SubmitChanges();
        }

        //获得用户离线消息并清空离线消息
        public List<Message> FetchUserMessage(string userName)
        {
            List<Message> newUserMessages = new List<Message>();
            var fetchedUser = (from u in dataContext.DatabaseUser
                               where u.UserName == userName
                               select u).Single();
            var fetchedUserMessage = from m in fetchedUser.DatabaseUserMessage
                                     select m;
            foreach (DatabaseUserMessage msg in fetchedUserMessage)
            {
                MessageType msgType = (MessageType)Enum.Parse(typeof(MessageType), msg.Type);
                Message newMessage = new Message(msgType, msg.Time, msg.Content, msg.Sender, msg.Receiver);
                newUserMessages.Add(newMessage.Clone() as Message);
            }
            foreach (DatabaseUserMessage msg in fetchedUserMessage)
            {
                dataContext.DatabaseUserMessage.DeleteOnSubmit(msg);
            }
            dataContext.SubmitChanges();
            return newUserMessages;
        }

        //创建群聊
        public void AddChat(ServerChat chat)
        {
            //创建聊天
            DatabaseGroupChat newChat = new DatabaseGroupChat()
            {
                ChatName = chat.ChatName,
                ChatNumber = chat.ChatNumber
            };
            //创建聊天用户列表，更新用户聊天列表
            foreach (string userName in chat.ChatUserNames)
            {
                DatabaseGroupChatUser newChatUser = new DatabaseGroupChatUser()
                {
                    UserName = userName
                };
                newChat.DatabaseGroupChatUser.Add(newChatUser);
                var currentUser = (from u in dataContext.DatabaseUser
                                   where u.UserName == userName
                                   select u).Single();
                DatabaseUserChat newUserChat = new DatabaseUserChat()
                {
                    Number = chat.ChatNumber
                };
                currentUser.DatabaseUserChat.Add(newUserChat);
            }
            dataContext.DatabaseGroupChat.InsertOnSubmit(newChat);
            dataContext.SubmitChanges();
        }

        //将用户添加到群聊
        public void AddChatUser(int chatNumber, string userName)
        {
            //将用户添加到聊天用户列表
            var selectedChat = (from c in dataContext.DatabaseGroupChat
                                where c.ChatNumber == chatNumber
                                select c).Single();
            DatabaseGroupChatUser newChatUser = new DatabaseGroupChatUser()
            {
                UserName = userName
            };
            selectedChat.DatabaseGroupChatUser.Add(newChatUser);
            //将群聊添加到用户群聊列表
            var selectedUser = (from u in dataContext.DatabaseUser
                                where u.UserName == userName
                                select u).Single();
            DatabaseUserChat newUserChat = new DatabaseUserChat()
            {
                Number = chatNumber
            };
            selectedUser.DatabaseUserChat.Add(newUserChat);
            dataContext.SubmitChanges();
        }

        //将用户移出群聊
        public void DeleteChatUser(int chatNumber, string userName)
        {
            //将用户移出聊天用户列表
            var selectedChat = (from c in dataContext.DatabaseGroupChat
                                where c.ChatNumber == chatNumber
                                select c).Single();
            var chatUserToDelete = (from u in selectedChat.DatabaseGroupChatUser
                                    where u.UserName == userName
                                    select u).Single();
            selectedChat.DatabaseGroupChatUser.Remove(chatUserToDelete);
            dataContext.DatabaseGroupChatUser.DeleteOnSubmit(chatUserToDelete);
            //将群聊添加到用户群聊列表
            var selectedUser = (from u in dataContext.DatabaseUser
                                where u.UserName == userName
                                select u).Single();
            var userChatToDelete = (from c in selectedUser.DatabaseUserChat
                                    where c.Number == chatNumber
                                    select c).Single();
            selectedUser.DatabaseUserChat.Remove(userChatToDelete);
            dataContext.DatabaseUserChat.DeleteOnSubmit(userChatToDelete);
            dataContext.SubmitChanges();
        }

        //查找群聊是否存在
        public bool ContainsChat(int chatNumber)
        {
            var selectedChat = from c in dataContext.DatabaseGroupChat
                               where c.ChatNumber == chatNumber
                               select c;
            if (selectedChat.Count() == 0)
            {
                return false;
            }
            return true;
        }

        //提取聊天信息
        public ServerChat GetChat(int chatNumber)
        {
            var selectedChat = (from c in dataContext.DatabaseGroupChat
                                where c.ChatNumber == chatNumber
                                select c).Single();
            string name = selectedChat.ChatName;
            int number = selectedChat.ChatNumber;
            var selectedUsers = from u in selectedChat.DatabaseGroupChatUser
                                select u;
            List<string> chatUserNames = new List<string>();
            foreach (DatabaseGroupChatUser user in selectedUsers)
            {
                chatUserNames.Add(user.UserName);
            }
            ServerChat newChat = new ServerChat(name, number, chatUserNames);
            return newChat;
        }

        //删除群聊
        public void RemoveChat(int chatNumber)
        {
            //step 1 删除DatabaseGroupChat相关
            //step 2 删除DatabaseUser相关
            var selectedChat = (from c in dataContext.DatabaseGroupChat
                                where c.ChatNumber == chatNumber
                                select c).Single();
            var chatUsers = from u in selectedChat.DatabaseGroupChatUser
                                select u;
            foreach (DatabaseGroupChatUser user in chatUsers)
            {
                var selectedUser = (from u in dataContext.DatabaseUser
                                    where u.UserName == user.UserName
                                    select u).Single();
                var selectedUserChat = (from c in selectedUser.DatabaseUserChat
                                        where c.Number == chatNumber
                                        select c).Single();
                dataContext.DatabaseUserChat.DeleteOnSubmit(selectedUserChat);
                dataContext.DatabaseGroupChatUser.DeleteOnSubmit(user);
            }
            dataContext.DatabaseGroupChat.DeleteOnSubmit(selectedChat);
            dataContext.SubmitChanges();
        }

        //添加聊天信息
        public void AddMessage(Message msg)
        {
            DatabaseMessage newMessage = new DatabaseMessage
            {
                Sender = msg.Sender,
                Receiver = msg.Receiver,
                Time = msg.SendTime,
                Type = msg.Type.ToString(),
                Content = msg.Content
            };
            dataContext.DatabaseMessage.InsertOnSubmit(newMessage);
            dataContext.SubmitChanges();
        }

        //查询聊天信息
        public List<Message> SearchMessage(string keyWord, string userName)
        {
            var selectedMessages = from m in dataContext.DatabaseMessage
                                   where (m.Sender == userName || m.Receiver == userName) && (m.Content.Contains(keyWord))
                                   orderby m.Time descending
                                   select m;
            List<Message> targetMessages = new List<Message>();
            foreach (DatabaseMessage msg in selectedMessages)
            {
                MessageType type = (MessageType)Enum.Parse(typeof(MessageType), msg.Type);
                string sender = msg.Sender;
                string receiver = msg.Receiver;
                string content = msg.Content;
                DateTime time = msg.Time;
                Message newMessage = new Message(type, time, sender, receiver, content);
                targetMessages.Add(newMessage);
            }
            return targetMessages;
        }

        //查询用户
        public List<ServerUser> SearchUser(string userName, string nickName, string gender, int age)
        {
            List<ServerUser> usersFound = new List<ServerUser>();
            if (this.ContainsUser(userName))
            {
                usersFound.Add(this.GetUser(userName));
            }
            //寻找昵称精确匹配
            var nickNameMatches = from u in dataContext.DatabaseUser
                                  where u.NickName == nickName
                                  select u;
            foreach (DatabaseUser user in nickNameMatches)
            {
                ServerUser newServerUser = new ServerUser()
                {
                    UserName = user.UserName,
                    NickName = user.NickName,
                    Age = user.Age,
                    Email = user.Email,
                    Gender = (GenderOption)Enum.Parse(typeof(GenderOption), user.Gender),
                    Password = user.Password,
                };
                usersFound.Add(newServerUser);
            }
            //寻找昵称粗匹配
            var nickNameRoughMatches = from u in dataContext.DatabaseUser
                                       where u.NickName.Contains(nickName)
                                       select u;
            foreach (DatabaseUser user in nickNameRoughMatches)
            {
                ServerUser newServerUser = new ServerUser()
                {
                    UserName = user.UserName,
                    NickName = user.NickName,
                    Age = user.Age,
                    Email = user.Email,
                    Gender = (GenderOption)Enum.Parse(typeof(GenderOption), user.Gender),
                    Password = user.Password,
                };
                usersFound.Add(newServerUser);
            }
            //按条件过滤
            if (gender.ToLower() != "unlimited")
            {
                List<ServerUser> usersToDelete = new List<ServerUser>();
                GenderOption filter = 
                    (GenderOption)Enum.Parse(typeof(GenderOption), gender);
                foreach (ServerUser user in usersFound)
                {
                    if (user.Gender != filter)
                    {
                        usersToDelete.Add(user);
                    }
                }
                foreach (ServerUser userToDelete in usersToDelete)
                {
                    usersFound.Remove(userToDelete);
                }
            }
            if (age != -1)
            {
                List<ServerUser> usersToDelete = new List<ServerUser>();
                foreach (ServerUser user in usersFound)
                {
                    if (user.Age != age)
                    {
                        usersToDelete.Add(user);
                    }
                }
                foreach (ServerUser userToDelete in usersToDelete)
                {
                    usersFound.Remove(userToDelete);
                }
            }
            return usersFound;
        }
    }
}
