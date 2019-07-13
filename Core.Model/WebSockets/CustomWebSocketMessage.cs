using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.WebSockets
{
    /// <summary>
    /// 自定义WebSocket消息
    /// </summary>
    public class CustomWebSocketMessage
    {
        public string Text { get; set; }
        public DateTime MessagDateTime { get; set; }
        public string Username { get; set; }
        public WSMessageType Type { get; set; }
    }
}
