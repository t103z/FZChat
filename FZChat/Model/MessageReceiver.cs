using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FZChat.Model.Utilities;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace FZChat.Model
{
    public class MessageReceiver : IMessageReceiver
    {
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<ConnectionLostEventArgs> ConnectionLost;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        private TcpListener listener;
        private TcpClient remoteClient;
        public static int ConnectionCount;
        public static bool OnService = false;
        private int _port;


        public MessageReceiver(int port)
        {
            _port = port;
            ((IMessageReceiver)this).StartListen(port);
        }

        //显示实现接口，在创建时就调用
        void IMessageReceiver.StartListen(int port)
        {
            Thread workTread = new Thread(new ThreadStart(ListenThreadMethod));
            workTread.Start();
        }

        //线程入口
        private void ListenThreadMethod()
        {
            //开始对本机IP和给定端口的监听
            IPAddress localIp = IPAddress.Parse(this.GetServerIPAddress());
            listener = new TcpListener(localIp, _port);
            listener.Start();
            OnService = true;
            Debug.WriteLine("Starts listening ip: {0} port: {1}", localIp.ToString(), _port);
            //等待挂起连接请求
            do
            {
                try
                {
                    Debug.WriteLine("Waiting for client...");
                    remoteClient = listener.AcceptTcpClient();
                    ConnectionCount++;
                    //启动新线程
                    Thread workThread = new Thread(new ThreadStart(ServiceClient));
                    workThread.Start();
                }
                catch
                {
                    continue;
                }
                if (ClientConnected != null)
                {
                    IPEndPoint endPoint = remoteClient.Client.RemoteEndPoint as IPEndPoint;
                    ClientConnected(this, new ClientConnectedEventArgs(endPoint));
                }
            } while (OnService);
        }

        private void ServiceClient()
        {
            Debug.WriteLine("Client connected!");
            bool keepConnect = true;
            //TcpClient复制引用还是值？
            TcpClient currentRemoteClient = remoteClient;
            Stream streamToClient = currentRemoteClient.GetStream();
            byte[] buffer = new byte[8192];
            //此循环监听一个客户端（通过一个线程）发来的消息
            while (OnService && keepConnect)
            {
                //客户端未调用Dispose()退出，抛出异常，若调用后退出，则Read()持续返回0
                try
                {
                    lock (streamToClient)
                    {
                        int bytesRead = streamToClient.Read(buffer, 0, 8192);
                        if (bytesRead == 0)
                        {
                            throw new Exception("客户端断开连接");
                        }
                        string msgString = Encoding.Unicode.GetString(buffer, 0, bytesRead);
                        if (MessageReceived != null)
                        {
                            MessageReceived(this, new MessageReceivedEventArgs(msgString, streamToClient, currentRemoteClient));
                        }
                    }
                }
                catch
                {
                    if (ConnectionLost != null)
                    {
                        IPEndPoint endPoint = remoteClient.Client.RemoteEndPoint as IPEndPoint;
                        ConnectionLost(this, new ConnectionLostEventArgs(endPoint.Address, endPoint.Port));
                    }
                    break;
                }
            }
        }

        public void StopListen()
        {
            try
            {
                listener.Stop();
                listener = null;
            }
            catch { }
        }

        public bool SendMessage(Message msg, Stream streamToClient)
        {
            string msgString = msg.ToString();
            byte[] buffer = Encoding.Unicode.GetBytes(msgString);
            try
            {
                streamToClient.Write(buffer, 0, buffer.Length);
                return true;
            }
            catch
            {
                return false;
                throw;
            }
        }

        //获取本机IP（待修改）
        private string GetServerIPAddress()
        {
            IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            if (addressList.Length < 1)
            {
                return string.Empty;
            }
            else if (addressList.Length < 2)
            {
                return addressList[0].ToString();
            }
            else if (addressList.Length < 3)
            {
                return addressList[1].ToString();
            }
            else
            {
                return addressList[2].ToString();
            }
        }
    }
}
