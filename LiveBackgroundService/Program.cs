using System.Net.Sockets;
using System.Text;

namespace LiveBackgroundService;

public static class Program
{
    public static async Task Main(string[] args)
    {
        int port = 6000;

        if (args != null && args.Length > 0)
        {
            try
            {
                port = int.Parse(args[0]);
            }
            catch
            {
                await Console.Out.WriteLineAsync("Specific port error");
            }
        }

        //var methods = new LocalMethods();
        //while (true)
        //{
        //    await Task.Delay(3000);
        //    var gameInfo = methods.DetectGameRunning();
        //    if (!string.IsNullOrEmpty(gameInfo))
        //    {
        //        await Console.Out.WriteLineAsync(gameInfo);
        //    }
        //}

        await StartBackgroundAsync(port);
    }

    public static async Task StartBackgroundAsync(int port)
    {
        var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        await client.ConnectAsync("localhost", port);
        await Console.Out.WriteLineAsync($"background connected server on " + port);

        var methods = new LocalMethods();

        while (true)
        {
            await Task.Delay(3000);

            var musiceInfo = methods.GetMusicInfo();
            if (!string.IsNullOrEmpty(musiceInfo))
            {
                await client.SendAsync(Encoding.UTF8.GetBytes(musiceInfo), SocketFlags.None);
            }


            var gameInfo = methods.DetectGameRunning();
            if (!string.IsNullOrEmpty(gameInfo))
            {
                await client.SendAsync(Encoding.UTF8.GetBytes(gameInfo), SocketFlags.None);
            }
        }
    }
}
