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
            dataContext.DatabaseUser.DeleteOnSubmit(dataContext.DatabaseUser.Single(u => u.UserName == userName));
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
                Password = selectedUser.Password
            };
            foreach (UserFriend userFriend in selectedUser.UserFriend)
            {
                newServerUser.FriendNames.Add(userFriend.UserName);
            }
            return newServerUser;
        }
    }
}
