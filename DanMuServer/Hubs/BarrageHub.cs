using System;
using System.Threading.Tasks;
using LiveCore.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace LiveServer.Hubs;

public class BarrageHub : Hub
{
    private readonly ILogger<BarrageHub> _logger;
    private readonly IBarrageCancellationService _barrageCancellationService;

    public BarrageHub(ILogger<BarrageHub> logger, IBarrageCancellationService barrageCancellationService)
    {
        _logger = logger;
        _barrageCancellationService = barrageCancellationService;
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("ConnectionId:{ConnectionId} connect server.", Context.ConnectionId);
        _barrageCancellationService.SetConnectionIdWithCancellationToken(Context.ConnectionId);
        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        _logger.LogInformation("ConnectionId:{ConnectionId} disconnect server.", Context.ConnectionId);
        _barrageCancellationService.Cancel(Context.ConnectionId);
        return Task.CompletedTask;
    }
}