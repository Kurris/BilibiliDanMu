using System;
using System.Threading;
using System.Threading.Tasks;
using LiveCore.Models;

namespace LiveCore.Interfaces;

public interface IBarrageService
{
    Task ReceiveBarrages(string connectionId, int roomId, Func<IServiceProvider, CancellationToken, Result, Task> onAction);
}