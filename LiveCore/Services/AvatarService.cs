using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveCore.Consts;

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

        public async Task<string> GetByBilibiliUserById(string id)
        {
            if (!_cache.ContainsKey(id))
            {
                var url = await _bilibiliApiService.GetUserAvatarFromSpaceHtmlAsync(id);
                _cache.TryAdd(id, url);
            }

            return _cache.GetValueOrDefault(id, BilibiliImageUrlConsts.NoFace);
        }

        public Dictionary<string, string> GetAll()
        {
            return _cache.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
