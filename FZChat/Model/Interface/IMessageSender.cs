using System.Net;

namespace FZChat.Model
{
    public interface IMessageSender
    {
        bool Connect(IPAddress ip, int port);
        bool SendMessage(Message msg);
        void SignOut();
    }
}
