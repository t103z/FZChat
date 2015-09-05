using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FZChat.Model
{
    public class MessageSender : IMessageSender
    {
        TcpClient client;
        Stream streamToServer;

        public bool Connect(IPAddress ip, int port)
        {
            try
            {
                client = new TcpClient();
                client.Connect(ip, port);
                streamToServer = client.GetStream();
                //应当把ServerResponse放到其他类中
                Thread workThread = new Thread(new ThreadStart(ServerResponse));
                workThread.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ServerResponse()
        {
            throw new NotImplementedException();
        }

        public bool SendMessage(Message msg)
        {
            try
            {
                byte[] buffer = Encoding.Unicode.GetBytes(msg.ToString());
                streamToServer.Write(buffer, 0, buffer.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SignOut()
        {
            if (streamToServer != null)
            {
                streamToServer.Dispose();
            }
            if (client != null)
            {
                client.Close();
            }
        }
    }
}
