using System;
using System.Threading;
using System.Threading.Tasks;
using LiveCore.Models;

namespace LiveCore.Interfaces
{
    public interface IBarrageConnectionProvider : IAsyncDisposable , IDisposable
    {
        /// <summary>
        /// 连接直播弹幕服务器
        /// </summary>
        /// <param name="roomId">房间号(可以为短号)</param>
        /// <param name="onResult">对结果处理</param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        Task<bool> ConnectAsync(int roomId, Action<Result> onResult, CancellationToken cancellation = default);

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        ValueTask DisconnectAsync();
    }
}
