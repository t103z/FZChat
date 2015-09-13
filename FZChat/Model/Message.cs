using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Model
{
    public enum MessageType
    {
        LOGIN,          //格式LOGIN|time|username|password
        REGISTER,       //格式REGISTER|time|username|password|nickname|gender|age|email
        PRIV,           //格式PRIV|time|sender|receiver|content
        GROUP,          //GROUP|time|sender|groupnumber|content
        JOIN,           //JOIN|time|username
        LEAVE,          //LEAVE|time|username
        ONLINELIST,     //ONLINELIST|time|SERVER|username1|username2|...
        FRIENDLIST,     //FRIENDLIST|time|SERVER|username1|nickname1|gender1|age1|email1|...
        FRIENDSEARCH,   //客户端发送FRIENDSEARCH|time|sender|username|nickname|gender|age 空缺写unlimited
                        //服务端发送FRIENDSEARCH|time|SERVER|username1|nickname1|gender1|age1|... 未找到写empty
        FRIENDREQUEST,  //FRIENDREQUEST|time|sender|receiver|content
        OK,             //OK|time
        CREATECHAT,     //CREATECHAT|time|sender|chatname|username1|username2|...
                        //服务端发送CREATECHAT|time|sender|chatnumber|username1|username2|...
        LEAVECHAT,      //LEAVECHAT|time|sender|chatnumber
        ERROR,
        INVALID         //INVALID|time
    }
    public class Message : ICloneable
    {
        private readonly MessageType _type;
        private readonly string _content = string.Empty;
        private readonly string _sender = string.Empty;
        private readonly string _receiver = string.Empty;
        private readonly DateTime _sendTime;

        public Message(MessageType type)
        {
            _type = type;
            _sendTime = SendTime;
        }

        public Message(MessageType type, string content)
        {
            _sendTime = DateTime.Now;
            _type = type;
            _content = content;
        }

        public Message(MessageType type, string sender, string receiver, string content)
        {
            _type = type;
            _content = content;
            _sender = sender;
            _receiver = receiver;
            _sendTime = DateTime.Now;
        }

        public Message(MessageType type, DateTime sendTime, string sender, string receiver, string content)
        {
            _type = type;
            _content = content;
            _sender = sender;
            _receiver = receiver;
            _sendTime = sendTime;
        }

        public Message(MessageType type, string sender, string content)
        {
            _type = type;
            _sender = sender;
            _content = content;
            _sendTime = DateTime.Now;
        }

        public Message(MessageType type, DateTime time, string sender, string content)
        {
            _type = type;
            _sendTime = time;
            _sender = sender;
            _content = content;
        }

        public Message(MessageType type, DateTime time)
        {
            _type = type;
            _sendTime = time;
        }

        public MessageType Type
        {
            get { return _type; }
        }

        public Message()
        {
            _type = MessageType.LOGIN;
        }

        public string Content
        {
            get { return _content; }
        }

        public DateTime SendTime
        {
            get { return _sendTime; }
        }

        public string Sender
        {
            get { return _sender; }
        }

        public string Receiver
        {
            get { return _receiver; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}|{1}|", this._type.ToString(), this._sendTime.ToString());
            if (!string.IsNullOrEmpty(_sender))
            {
                sb.AppendFormat("{0}|", this._sender);
            }
            if (!string.IsNullOrEmpty(_receiver))
            {
                sb.AppendFormat("{0}|", this._receiver);
            }
            if (!string.IsNullOrEmpty(_content))
            {
                sb.AppendFormat("{0}|", this._content);
            }
            return sb.ToString();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
