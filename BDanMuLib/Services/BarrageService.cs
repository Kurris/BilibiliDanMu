using System;
using System.Threading;
using System.Threading.Tasks;
using BDanMuLib.Interfaces;
using BDanMuLib.Models;
using Microsoft.Extensions.Logging;

namespace BDanMuLib.Services
{
    internal class BarrageService : IBarrageService
    {
        private readonly IBarrageConnectionProvider _barrageProvider;
        private readonly ILogger<BarrageService> _logger;
        private readonly IBarrageCancellationService _barrageCancellationService;

        public BarrageService(IBarrageConnectionProvider barrageProvider,
            ILogger<BarrageService> logger,
            IBarrageCancellationService barrageCancellationService)
        {
            _barrageProvider = barrageProvider;
            _logger = logger;
            _barrageCancellationService = barrageCancellationService;
        }


        public async Task ReceiveBarrages(string connectionId, int roomId, Func<CancellationToken, Result, Task> OnAction)
        {
            var cancellationTokenSource = _barrageCancellationService.Get(connectionId);

            _logger.LogInformation("Get current connectionId's cancellationToken . {ConnectionId}:{RoomId}", connectionId, roomId);

            //长任务执行
            await Task.Factory.StartNew(async () =>
            {
                var currentConnectionId = connectionId;
                var currentRoomId = roomId;

                await _barrageProvider.ConnectAsync(roomId, async result =>
                {
                    await OnAction(cancellationTokenSource.Token, result);

                }, cancellationTokenSource.Token);

                _logger.LogInformation("{ConnectionId}:{RoomId} receive barrages end!", connectionId, roomId);

            }, TaskCreationOptions.LongRunning);

            _logger.LogInformation("{ConnectionId}:{RoomId} Long task receive barrages begin!", connectionId, roomId);
        }
    }
}
