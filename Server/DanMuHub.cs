using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Server
{
    public class DanMuHub : Hub
    {
        public static ConcurrentBag<string> OnLineUsers = new ConcurrentBag<string>();


        public override Task OnConnectedAsync()
        {
            OnLineUsers.Add(Context.ConnectionId);
            Console.WriteLine($"{Context.ConnectionId}已连接。");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            OnLineUsers = new ConcurrentBag<string>();
            OnLineUsers.Where(x => x != Context.ConnectionId).ToList().ForEach(x =>
            {
                OnLineUsers.Add(x);
            });
            Console.WriteLine($"{Context.ConnectionId}已断开。");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
