using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using LiveCore.Interfaces;

namespace LiveCore.Services
{
    public class BarrageCancellationService : IBarrageCancellationService
    {
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _sources = new();

        public bool Cancel(string connectionId)
        {
            if (_sources.TryGetValue(connectionId, out var cancellationToken) && cancellationToken != null)
            {
                cancellationToken.Cancel();

            }
            _sources.Remove(connectionId, out _);
            return true;
        }

        public void SetConnectionIdWithCancellationToken(string connectionId)
        {
            if (_sources.ContainsKey(connectionId))
            {
                _sources.Remove(connectionId, out _);
            }

            _sources.TryAdd(connectionId, null);
        }

        public CancellationTokenSource Get(string connectionId)
        {
            //获取时才做初始化
            _sources[connectionId] = new CancellationTokenSource();
            return _sources[connectionId];
        }

        public int ConnectionCount() => _sources.Count;

        public bool ExistsCancelToken(string connectionId)
        {
            //同步锁 or 负载后使用分布式锁
            if (_sources.TryGetValue(connectionId, out var cancellationToken))
            {
                return cancellationToken != null;
            }
            return false;
        }
    }
}
