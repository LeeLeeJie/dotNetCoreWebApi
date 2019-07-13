using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace Core.Model.WebSockets
{
   public class CustomWebSocket
    {
        public WebSocket WebSocket { get; set; }
        public string Username { get; set; }
    }
}
