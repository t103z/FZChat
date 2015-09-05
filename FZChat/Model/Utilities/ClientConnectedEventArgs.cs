﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FZChat.Model.Utilities
{
    public class ClientConnectedEventArgs : EventArgs
    {
        public IPEndPoint EndPoint { get; set; }
        
        public ClientConnectedEventArgs(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
        }
    }
}
