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

        public void AddUser(ServerUser user)
        {
            DatabaseUser newUser = new DatabaseUser()
            {
                UserName = user.UserName,
                NickName = user.NickName,
                Gender = user.Gender.ToString(),
                Email = user.Email,
                Age = user.Age,
            };
            foreach (string username in user.FriendNames)
            {
                UserFriend newFriend = new UserFriend() { UserName = username };
                newUser.UserFriend.Add(newFriend);
            }
            dataContext.DatabaseUser.InsertOnSubmit(newUser);
            dataContext.SubmitChanges();
        }
        public void RemoveUser(string userName)
        {
            var selectedUser = (from u in dataContext.DatabaseUser
                                where u.UserName == userName
                                select u).Single();
            if (selectedUser.UserFriend.Count() > 0)
            {
                var selectedUserFriend = from f in selectedUser.UserFriend
                                         select f;
                foreach (UserFriend userFriend in selectedUserFriend)
                {
                    dataContext.UserFriend.DeleteOnSubmit(userFriend);
                }
            }
            if (selectedUser.DatabaseUserMessage.Count() > 0)
            {
                var selectedMessage = from m in selectedUser.DatabaseUserMessage
                                      select m;
                foreach (DatabaseUserMessage msg in selectedMessage)
                {
                    dataContext.DatabaseUserMessage.DeleteOnSubmit(msg);
                }
            }
            dataContext.DatabaseUser.DeleteOnSubmit(selectedUser);
            dataContext.SubmitChanges();
        }
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
    }
}
