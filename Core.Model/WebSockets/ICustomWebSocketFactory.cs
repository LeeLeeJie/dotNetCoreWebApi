using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.WebSockets
{
    //CustomWebSocketFactory负责收集连接的WebSockets列表
    public interface ICustomWebSocketFactory
    {
        void Add(CustomWebSocket uws);
        void Remove(string username);
        List<CustomWebSocket> All();
        List<CustomWebSocket> Others(CustomWebSocket client);
        CustomWebSocket Client(string username);
    }
}
