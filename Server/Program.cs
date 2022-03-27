using System;
using BDanMuLib;
using System.Linq;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var url = "http://localhost:5000";
            using (WebApp.Start(url, builder =>
            {
                builder.Map("/danmu", options =>
                {
                    options.UseCors(CorsOptions.AllowAll);
                });

                builder.MapSignalR();
            }))
            {
                var hub = GlobalHost.ConnectionManager.GetHubContext<DanMuHub>();
                DanMuCore core = new();
                core.ReceiveMessage += Core_ReceiveMessage;

                while (true)
                {
                    DanMuHub.OnLineUsers.Values.ToList().ForEach(x =>
                    {
                        var client = hub.Clients.Client(x);
                    });

                }
            }
        }

        private static void Core_ReceiveMessage(BDanmuLib.Models.MessageType messageType, object obj)
        {
            throw new NotImplementedException();
        }
    }
}
