using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Model
{
    public enum MessageType
    {
        LOGIN,
        REGISTER,
        PRIV,
        GROUP,
        LIST,
        FRIEND,
        FRIENDREQUEST,
        OK,
        CHATROOM,
        ERROR,
        INVALID
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
        }

        public Message(MessageType type, DateTime time, string sender, string content)
        {
            _type = type;
            _sendTime = time;
            _sender = sender;
            _content = content;
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
