using System.Threading.Tasks;
using LiveCore.Models;
using LiveCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace LiveServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StreamerController : ControllerBase
{
    private readonly BilibiliApiService _bilibiliApiService;

    public StreamerController(BilibiliApiService bilibiliApiService)
    {
        _bilibiliApiService = bilibiliApiService;
    }


    [HttpGet]
    public async Task<ApiResult<StreamerInfo>> GetStreamerInfoByRoomId(int roomId)
    {
        var roomInfo = await _bilibiliApiService.GetRoomInfoAsync(roomId);
        if (roomInfo == null)
        {
            return new ApiResult<StreamerInfo>()
            {
                Code = 404
            };
        }
        var info = await _bilibiliApiService.GetStreamerInfoAsync(roomInfo.Uid);
        info.RoomInfo = roomInfo;

        return new ApiResult<StreamerInfo>()
        {
            Data = info
        };
    }
}