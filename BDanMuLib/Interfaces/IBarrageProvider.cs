using System;
using System.Threading;
using System.Threading.Tasks;
using BDanMuLib.Models;

namespace BDanMuLib.Interfaces
{
    public interface IBarrageProvider
    {
        /// <summary>
        /// 连接直播弹幕服务器
        /// </summary>
        /// <param name="roomId">房间号(可以为短号)</param>
        /// <param name="onReceive">对结果处理</param>
        /// <returns></returns>
        Task ConnectAsync(int roomId, Action<Result> OnResult, CancellationToken cancellation = default);

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        ValueTask DisconnectAsync();
    }
}
