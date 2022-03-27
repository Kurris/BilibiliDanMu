using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using BDanmuLib.Models;
using BDanMuLib;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace DanMuServer
{
    public class DanMuHub : Hub
    {
        private static string _id;
        private static DanMuCore _danmu;
        private readonly IConfiguration _configuration;

        public DanMuHub(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        static DanMuHub()
        {
        }


        public override async Task OnConnectedAsync()
        {
            _id = Context.ConnectionId;
            if (_danmu != null)
            {
                _danmu.Disconnect();
            }
            _danmu = new DanMuCore();
            await _danmu.ConnectAsync(int.Parse(_configuration.GetSection("RoomId").Value));

            Console.WriteLine(Context.ConnectionId + "成功连接");
            await base.OnConnectedAsync();


            _danmu.ReceiveMessage += (type, obj) =>
            {
                if (!string.IsNullOrEmpty(_id))
                {
                    switch (type)
                    {
                        case MessageType.DANMU_MSG:
                            Clients.Client(_id).SendAsync("addDanmu", obj);
                            break;
                        case MessageType.INTERACT_WORD:
                            Clients.Client(_id).SendAsync("joinRoom", obj);
                            break;
                        case MessageType.WATCHED_CHANGE:
                            Clients.Client(_id).SendAsync("watched", obj);
                            break;
                        case MessageType.NONE:
                            Clients.Client(_id).SendAsync("hot", obj);
                            break;
                        default:
                            break;
                    }
                }
            };

            while (true)
            {
                await Task.Delay(1000);
                if (string.IsNullOrEmpty(_id))
                {
                    break;
                }
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _id = string.Empty;
            _danmu.Disconnect();
            _danmu = null;
            Console.WriteLine(Context.ConnectionId + "断开连接");
            return base.OnDisconnectedAsync(exception);
        }


        public void Disconnect()
        {
            _id = string.Empty;
        }
    }
}
