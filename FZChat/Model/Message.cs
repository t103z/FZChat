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
        OK,
        ERROR,
        INVALID
    }
    public class Message
    {
        private readonly MessageType _type;
        private readonly string _content = string.Empty;
        private readonly string _sender = string.Empty;
        private readonly string _receiver = string.Empty;
        private readonly DateTime _sendTime;

        public Message(MessageType type, string content, string sender, string receiver)
        {
            _type = type;
            _content = content;
            _sender = sender;
            _receiver = receiver;
            _sendTime = DateTime.Now;
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

        public string sender
        {
            get { return _sender; }
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
            sb.AppendFormat("{0}|", this._content);
            return sb.ToString();
        }
    }
}
