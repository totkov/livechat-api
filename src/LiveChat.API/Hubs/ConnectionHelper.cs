using System;
using System.Collections.Concurrent;

namespace LiveChat.API.Hubs
{
    public static class ConnectionHelper
    {
        private static ConcurrentDictionary<int, string> _connections = new ConcurrentDictionary<int, string>();

        public static void AddConnection(string connectionId, int userId)
        {
            _connections.TryAdd(userId, connectionId);
        }

        public static void RemoveConnection(int userId)
        {
            _connections.TryRemove(userId, out string connectionId);
        }

        public static ConcurrentDictionary<int, string> GetAllConnections()
        {
            return _connections;
        }

        public static int GetConnectionsCount()
        {
            return _connections.Count;
        }
    }
}
