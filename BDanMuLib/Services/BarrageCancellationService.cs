using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using BDanMuLib.Interfaces;

namespace BDanMuLib.Services
{
    public class BarrageCancellationService : IBarrageCancellationService
    {
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _sources = new();

        public bool Cancel(string connectionId)
        {
            if (_sources.ContainsKey(connectionId))
            {
                _sources[connectionId].Cancel();
                _sources.Remove(connectionId, out _);
                return true;
            }
            return false;
        }

        public void SetConnectionIdWithCancellationToken(string connectionId)
        {
            if (_sources.ContainsKey(connectionId))
            {
                _sources[connectionId].Cancel();
                _sources.Remove(connectionId, out _);
            }

            _sources.TryAdd(connectionId, new CancellationTokenSource());
        }

        public CancellationTokenSource Get(string connectionId)
        {
            if (!_sources.ContainsKey(connectionId))
            {
                _sources.TryAdd(connectionId, new CancellationTokenSource());
            }
            return _sources[connectionId];
        }

        public int ConnectionCount() => _sources.Count;
    }
}
