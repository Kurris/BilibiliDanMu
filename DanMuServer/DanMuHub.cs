using System;
using System.Threading;
using System.Threading.Tasks;
using BDanMuLib;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace DanMuServer
{
    public class DanMuHub : Hub
    {

        public override async Task OnConnectedAsync()
        {
            await Console.Out.WriteLineAsync($"{Context.ConnectionId}:Connect server successfully");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Console.Out.WriteLineAsync($"{Context.ConnectionId} Disconnect room");
        }


        public async Task Start(int roomId)
        {
            RoomCache.Put(Context.ConnectionId, roomId);
            await Console.Out.WriteLineAsync($"{Context.ConnectionId} start");
        }
    }
}
