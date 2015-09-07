﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FZChat.Model.Utilities;

namespace FZChat.Model
{
    public class MessageSender : IMessageSender
    {
        TcpClient client;
        Stream streamToServer;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ConnectionLostEventArgs> ConnectionLost;

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
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead = streamToServer.Read(buffer, 0, 8192);
                    if (bytesRead == 0)
                    {
                        throw new Exception("Connection lost!");
                    }
                    else
                    {
                        string msgString = Encoding.Unicode.GetString(buffer);
                        if (MessageReceived != null)
                        {
                            MessageReceived(this, new MessageReceivedEventArgs(msgString));
                        }
                    }
                }
                catch
                {
                    if (ConnectionLost != null)
                    {
                        ConnectionLost(this, new ConnectionLostEventArgs());
                    }
                }
                
                
            }
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
