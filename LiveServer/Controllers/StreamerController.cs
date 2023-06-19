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
    public async Task<ApiResult<object>> GetMasterByRoomId(int roomId)
    {
        var roomInfo = await _bilibiliApiService.GetRoomInfoAsync(roomId);
        var info = await _bilibiliApiService.GetStreamerInfoAsync(roomInfo.Uid);

        return new ApiResult<object>()
        {
            Data = new
            {
                room = roomInfo,
                user = info
            }
        };
    }
}