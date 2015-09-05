using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FZChat.Model.Utilities;

namespace FZChat.Model
{
    public class Server
    {
        private MessageReceiver receiver;
        public List<OnlineUser> OnlineUsers;
        private UserDatabase database;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived
        {
            add
            {
                receiver.MessageReceived += value;
            }
            remove
            {
                receiver.MessageReceived -= value;
            }
        }

        public event EventHandler<ConnectionLostEventArgs> ConnectionLost
        {
            add
            {
                receiver.ConnectionLost += value;
            }
            remove
            {
                receiver.ConnectionLost -= value;
            }
        }

        public event EventHandler<ClientConnectedEventArgs> ClientConnected
        {
            add
            {
                receiver.ClientConnected += value;
            }
            remove
            {
                receiver.ClientConnected -= value;
            }
        }

        public Server(int port)
        {
            this.receiver = new MessageReceiver(port);
            receiver.MessageReceived += new EventHandler<MessageReceivedEventArgs>(ProcessMessage);
        }

        private void ProcessMessage(object sender, MessageReceivedEventArgs e)
        {
            string originalString = e.Content;
            string[] tokens = originalString.Split(new char[] { '|' });
            Message receivedMessage = new Message();
        }

        private Message GenerateMessage(string msgString)
        {
            string[] tokens = msgString.Split(new char[] { '|' });
        }
    }
}
