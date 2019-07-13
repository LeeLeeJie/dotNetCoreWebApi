using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model.WebSockets
{
   public class CustomWebSocketFactory : ICustomWebSocketFactory
    {
        List<CustomWebSocket> List;

        public CustomWebSocketFactory()
        {
            List = new List<CustomWebSocket>();
        }
        public void Add(CustomWebSocket uws)
        {
            List.Add(uws);
        }

        public List<CustomWebSocket> All()
        {
            return List;
        }

        public CustomWebSocket Client(string username)
        {
            return List.FirstOrDefault(c => c.Username == username);
        }

        public List<CustomWebSocket> Others(CustomWebSocket client)
        {
            return List.Where(c => c.Username != client.Username).ToList();
        }

        public void Remove(string username)
        {
            List.Remove(Client(username));
        }
    }
}
