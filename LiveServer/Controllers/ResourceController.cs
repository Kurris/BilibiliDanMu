using System.Collections.Generic;
using LiveCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace LiveServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly AvatarService _avatarService;

        public ResourceController(AvatarService avatarService)
        {
            _avatarService = avatarService;
        }

        /// <summary>
        /// 获取临时保存的头像资源
        /// </summary>
        /// <returns></returns>
        [HttpGet("avatars")]
        public ApiResult<Dictionary<string, string>> GetAvatarTempCaches()
        {
            return new ApiResult<Dictionary<string, string>>()
            {
                Data = _avatarService.GetAll()
            };
        }
    }
}
