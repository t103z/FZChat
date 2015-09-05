using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Model
{
    public class ServerUser : IUser
    {
        public string UserName { get; set; }
        public string NickName { get; set; }
        public GenderOption Gender { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public int Head { get; set; }
        public string Password { get; set; }
        public List<string> FriendNames;
    }
}
