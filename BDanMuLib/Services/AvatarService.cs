using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiveCore.Services
{
    public class AvatarService
    {
        private readonly ConcurrentDictionary<string, string> _cache = new();
        private readonly BilibiliApiService _bilibiliApiService;

        public AvatarService(BilibiliApiService bilibiliApiService)
        {
            _bilibiliApiService = bilibiliApiService;
        }

        public async Task<string> GetByBilibiliUserId(string id)
        {

            if (!_cache.ContainsKey(id))
            {
                var url = await _bilibiliApiService.GetUserAvatarFromSpaceHtmlAsync(id);
                _cache.TryAdd(id, url);
            }

            return _cache.GetValueOrDefault(id);
        }
    }
}
