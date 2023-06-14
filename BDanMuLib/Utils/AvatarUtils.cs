using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BDanMuLib.Utils
{
    public class AvatarUtils
    {

        private readonly static Dictionary<string, string> _cache = new Dictionary<string, string>();
        
        private AvatarUtils()
        {
        }

        public static async Task<string> Get(string mid)
        {
            if (!_cache.ContainsKey(mid))
            {
                var url = await RequestUtils.GetUserAvatarFromSpaceHtmlAsync(mid);
                _cache.Add(mid, url);
            }

            return _cache.GetValueOrDefault(mid);
        }
    }
}

