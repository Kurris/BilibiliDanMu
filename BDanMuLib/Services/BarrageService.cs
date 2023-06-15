using System;
using System.Threading;
using System.Threading.Tasks;
using BDanMuLib.Interfaces;
using BDanMuLib.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BDanMuLib.Services
{
    internal class BarrageService : IBarrageService
    {
        private readonly ILogger<BarrageService> _logger;
        private readonly IBarrageCancellationService _barrageCancellationService;

        public BarrageService(ILogger<BarrageService> logger,
            IBarrageCancellationService barrageCancellationService)
        {
            _logger = logger;
            _barrageCancellationService = barrageCancellationService;
        }


        public async Task ReceiveBarrages(string connectionId, int roomId, Func<CancellationToken, Result, Task> OnAction)
        {
            if (_barrageCancellationService.ExistsCancelToken(connectionId))
            {
                return;
            }

            _logger.LogInformation("{ConnectionId} request to receive room:{RoomId} barrages.", connectionId, roomId);
            var cancellationTokenSource = _barrageCancellationService.Get(connectionId);

            _logger.LogInformation("{ConnectionId} get {RoomId}'s cancellationToken.", connectionId, roomId);

            //长任务执行
            await Task.Factory.StartNew(async () =>
            {
                var currentConnectionId = connectionId;
                var currentRoomId = roomId;

                //从根容器获取scope
                using var asyncScope = InternalApp.ApplicationServices.CreateAsyncScope();
                await using var barrageProvider = asyncScope.ServiceProvider.GetService<IBarrageConnectionProvider>();
                await barrageProvider.ConnectAsync(roomId, async result =>
                {
                    await OnAction(cancellationTokenSource.Token, result);

                }, cancellationTokenSource.Token);

                _logger.LogInformation("{ConnectionId}:{RoomId} receive barrages end!", connectionId, roomId);

            }, TaskCreationOptions.LongRunning);

            _logger.LogInformation("{ConnectionId} a Long task start to receive barrages begin!", connectionId);
        }
    }
}
