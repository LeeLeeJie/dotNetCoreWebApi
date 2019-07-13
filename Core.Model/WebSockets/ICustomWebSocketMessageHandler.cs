using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.WebSockets
{
    //CustomWebSocketMessageHandler包含有关消息的逻辑（即在连接时需要发送任何消息以及如何对传入消息作出反应）
   public interface ICustomWebSocketMessageHandler
    {
        Task SendInitialMessages(CustomWebSocket userWebSocket);
        Task HandlerMessage(WebSocketReceiveResult result, byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
        Task BroadcastOthers(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
        Task BroadcastAll(byte[] buffer, CustomWebSocket userWebSocket, ICustomWebSocketFactory wsFactory);
    }
}
