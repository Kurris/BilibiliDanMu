using System;
using System.Collections.Generic;
using System.Linq;

namespace BDanMuLib
{
    public class RoomCache
    {
        private readonly static Dictionary<string, int> _cache = new Dictionary<string, int>();

        public RoomCache()
        {
        }

        public static void Put(string key, int roomId)
        {
            var existsRoomId = _cache.ContainsValue(roomId);
            if (existsRoomId)
            {
                _cache.Remove(_cache.First(x => x.Value == roomId).Key);
            }
            _cache.Add(key, roomId);
        }

        public static bool Any()
        {
            return _cache.Any();
        }

        public static IEnumerable<string> GetAllConnectionIds()
        {
            return _cache.Select(x => x.Key);
        }

        public static void Delete(string key)
        {
            _cache.Remove(key);
        }

        public static int GetRoomId(string key)
        {
            if (_cache.TryGetValue(key, out var roomId))
            {
                return roomId;
            }
            return 0;
        }

        public static (string connectId, int roomId) ShiftRoomId()
        {
            var first = _cache.First();
            _cache.Remove(first.Key, out var roomId);
            return (first.Key, roomId);
        }
    }
}

