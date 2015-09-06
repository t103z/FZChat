using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using FZChatServer.Orm;

namespace FZChat.Model
{
    public class OnlineUser : ServerUser
    {
        public IPEndPoint EndPoint { get; set; }
        public OnlineUser(ServerUser serverUser, IPEndPoint endpoint)
        {
            this.UserName = serverUser.UserName;
            this.NickName = serverUser.NickName;
            this.Gender = serverUser.Gender;
            this.Head = serverUser.Head;
            this.Password = serverUser.Password;
            this.Age = serverUser.Age;
            this.Email = serverUser.Email;
            this.EndPoint = endpoint;
        }
    }
}
