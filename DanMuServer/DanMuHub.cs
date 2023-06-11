using System;
using System.Threading;
using System.Threading.Tasks;
using BDanmuLib.Models;
using BDanMuLib;
using Microsoft.AspNetCore.SignalR;

namespace DanMuServer
{
    public class DanMuHub : Hub
    {
        private static string _id;
        private static CancellationTokenSource _source;

        public override async Task OnConnectedAsync()
        {
            _id = Context.ConnectionId;
            _source = new CancellationTokenSource();
            await Console.Out.WriteLineAsync($"{Context.ConnectionId}:connect server successfully");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _source.Cancel();
            await DanMuCore.DisconnectAsync();
            _id = null;
            await Console.Out.WriteLineAsync($"{Context.ConnectionId} disconnect room");
        }


        public async Task Start(int roomId)
        {
            await DanMuCore.ConnectAsync(roomId, async result =>
            {
                if (!_source.IsCancellationRequested)
                {
                    await Clients.Client(_id).SendAsync(result.Type.ToString(), result.Info);
                }
            }, _source.Token);

            await Console.Out.WriteLineAsync("done");
        }
    }
}
