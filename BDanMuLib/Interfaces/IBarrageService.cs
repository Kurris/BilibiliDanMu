
using System;
using System.Threading;
using System.Threading.Tasks;
using BDanMuLib.Models;

namespace BDanMuLib.Interfaces
{
    public interface IBarrageService
    {
        Task ReceiveBarrages(string connectionId, int roomId, Func<CancellationToken, Result, Task> OnAction);
    }
}
