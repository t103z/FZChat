using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using FZChatServer.Orm;
using System.Net.Sockets;

namespace FZChat.Model
{
    public class OnlineUser : ServerUser
    {
        public NetworkStream StreamToRemote { get; set; }
        public OnlineUser(ServerUser serverUser, NetworkStream stream)
        {
            this.UserName = serverUser.UserName;
            this.NickName = serverUser.NickName;
            this.Gender = serverUser.Gender;
            this.Head = serverUser.Head;
            this.Password = serverUser.Password;
            this.Age = serverUser.Age;
            this.Email = serverUser.Email;
            this.StreamToRemote = stream;
        }
    }
}
