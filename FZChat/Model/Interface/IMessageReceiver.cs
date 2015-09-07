using FZChat.Model.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Model
{
    public interface IMessageReceiver
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<ConnectionLostEventArgs> ConnectionLost;
        event EventHandler<ClientConnectedEventArgs> ClientConnected;
        void StartListen(int port);
        void StopListen();
    }
}
