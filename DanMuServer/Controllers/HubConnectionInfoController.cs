using BDanMuLib.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DanMuServer.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class HubConnectionInfoController : ControllerBase
    {
        private readonly ILogger<HubConnectionInfoController> _logger;
        private readonly IBarrageCancellationService _barrageCancellationService;

        public HubConnectionInfoController(ILogger<HubConnectionInfoController> logger,
            IBarrageCancellationService barrageCancellationService)
        {
            _logger = logger;
            _barrageCancellationService = barrageCancellationService;
        }


        [HttpGet("count")]
        public ApiResult<int> GetConnectionCount()
        {
            return new ApiResult<int>()
            {
                Data = _barrageCancellationService.ConnectionCount()
            };
        }
    }
}
