using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Model
{
    public class ServerUser : IUser, ICloneable
    {
        public string UserName { get; set; }
        public string NickName { get; set; }
        public GenderOption Gender { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public int Head { get; set; }
        public string Password { get; set; }
        public List<string> FriendNames;

        public object Clone()
        {
            ServerUser newServerUser = new ServerUser();
            newServerUser.UserName = this.UserName;
            newServerUser.NickName = this.NickName;
            newServerUser.Gender = this.Gender;
            newServerUser.Age = this.Age;
            newServerUser.Email = this.Email;
            newServerUser.Head = this.Head;
            newServerUser.Password = this.Password;
            newServerUser.FriendNames = new List<string>();
            foreach (string name in this.FriendNames)
            {
                newServerUser.FriendNames.Add(name);
            }
            return newServerUser;
        }
    }
}
