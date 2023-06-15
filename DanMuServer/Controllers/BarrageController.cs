using System.Threading.Tasks;
using BDanMuLib.Interfaces;
using DanMuServer.Hubs;
using DanMuServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DanMuServer.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BarrageController : ControllerBase
    {
        private readonly IBarrageService _barrageService;
        private readonly ILogger<BarrageController> _logger;
        private readonly IHubContext<BarrageHub> _barrageHubContext;
        private readonly IBarrageCancellationService _barrageCancellationService;

        public BarrageController(ILogger<BarrageController> logger,
            IHubContext<BarrageHub> barrageHubContext,
            IBarrageCancellationService barrageCancellationService,
            IBarrageService barrageService)
        {
            _barrageService = barrageService;
            _logger = logger;
            _barrageHubContext = barrageHubContext;
            _barrageCancellationService = barrageCancellationService;
        }

        [HttpPost("receive")]
        public async Task ReceiveBarrages(ReceiveInputDto input)
        {
            var connectionId = input.ConnectionId;
            var roomId = input.RoomId;

            await _barrageService.ReceiveBarrages(connectionId, roomId, async (cancelToken, result) =>
            {
                try
                {
                    await _barrageHubContext.Clients.Client(connectionId).SendAsync(result.Type.ToString(), result.Info, cancelToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("{connectionId}:{roomId} Hub send event canceled.", connectionId, roomId);
                }
            });
        }
    }
}
