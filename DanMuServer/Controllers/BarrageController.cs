using System.Threading.Tasks;
using LiveCore.Enums;
using LiveCore.Interfaces;
using LiveCore.Services;
using LiveServer.Hubs;
using LiveServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace LiveServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BarrageController : ControllerBase
{
    private readonly IBarrageService _barrageService;
    private readonly ILogger<BarrageController> _logger;

    public BarrageController(ILogger<BarrageController> logger,
        IBarrageService barrageService)
    {
        _barrageService = barrageService;
        _logger = logger;
    }

    [HttpPost("receive")]
    public async Task ReceiveBarrages(ReceiveInputDto input)
    {
        var connectionId = input.ConnectionId;
        var roomId = input.RoomId;

        await _barrageService.ReceiveBarrages(connectionId, roomId, async (sp, cancelToken, result) =>
        {
            try
            {
                var hub = sp.GetService<IHubContext<BarrageHub>>()!;
                await hub.Clients.Client(connectionId).SendAsync(result.Type.ToString(), result.Info, cancelToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("{connectionId} Hub send to {roomId} event was canceled.", connectionId, roomId);
            }
        });
    }
}