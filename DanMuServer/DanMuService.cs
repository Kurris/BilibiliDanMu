using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BDanmuLib.Enums;
using BDanMuLib;
using BDanMuLib.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace DanMuServer
{
    public class DanMuService : IHostedService
    {
        private readonly IHubContext<DanMuHub> _hubContext;

        public DanMuService(IHubContext<DanMuHub> hubContext)
        {
            this._hubContext = hubContext;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(async () =>
              {
                  while (true)
                  {
                      if (RoomCache.Any())
                      {
                          var (connectionId, roomId) = RoomCache.ShiftRoomId();

                          await Task.Factory.StartNew(async () =>
                          {
                              await new BilibiliBarrage().ConnectAsync(roomId, async result =>
                              {
                                  await _hubContext.Clients.Client(connectionId).SendAsync(result.Type.ToString(), result.Info, cancellationToken);
                                  if (result.Type == MessageType.SUPER_CHAT_MESSAGE)
                                  {
                                      _ = Task.Run(() =>
                                      {
                                          lock (this)
                                          {
                                              _ = _hubContext.Clients.Client(connectionId).SendAsync("READ_SC", (result.Info as SuperChatInfo).SpeakText, cancellationToken);
                                          }
                                      });
                                  }
                              }, cancellationToken);
                          });
                      }

                      await Task.Delay(5000, cancellationToken);
                  }
              }, TaskCreationOptions.LongRunning);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

