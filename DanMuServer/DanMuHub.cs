using System;
using System.Threading.Tasks;
using BDanmuLib.Models;
using BDanMuLib;
using Microsoft.AspNetCore.SignalR;

namespace DanMuServer
{
    public class DanMuHub : Hub
    {
        private static DanMuCore _danmu;
        private static string _id;
        private static IHubCallerClients _clients;

        public override async Task OnConnectedAsync()
        {
            _id = Context.ConnectionId;
            await Console.Out.WriteLineAsync($"{Context.ConnectionId}:connect server successfully");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_danmu != null)
            {
                _danmu.Disconnect();
                _danmu = null;
            }
            _clients = null;
            _id = null;
            await Console.Out.WriteLineAsync($"{Context.ConnectionId} disconnect room");
        }


        public async Task Start(object roomId)
        {
            if (_danmu != null)
            {
                _danmu.Disconnect();
            }
            _clients = Clients;
            _danmu = new DanMuCore();
            await Task.Factory.StartNew(() =>
              {
                  _danmu.ReceiveMessage += (type, obj) =>
                        {
                            switch (type)
                            {
                                case MessageType.DANMU_MSG:
                                    _clients?.Client(_id).SendAsync("addDanmu", obj);
                                    break;
                                case MessageType.INTERACT_WORD:
                                    _clients?.Client(_id).SendAsync("joinRoom", obj);
                                    break;
                                case MessageType.WATCHED_CHANGE:
                                    _clients?.Client(_id).SendAsync("watched", obj);
                                    break;
                                case MessageType.NONE:
                                    _clients?.Client(_id).SendAsync("hot", obj);
                                    break;
                                case MessageType.ENTRY_EFFECT:
                                    _clients?.Client(_id).SendAsync("entry_effect", obj.ToString());
                                    break;
                                case MessageType.SUPER_CHAT_MESSAGE:
                                    _clients?.Client(_id).SendAsync("sc", obj);
                                    break;
                                default:
                                    break;
                            }
                        };
              });

            await _danmu.ConnectAsync(int.Parse(roomId.ToString()));
            await Console.Out.WriteLineAsync($"{Context.ConnectionId} connect room successfully: {roomId}");
        }
    }
}
