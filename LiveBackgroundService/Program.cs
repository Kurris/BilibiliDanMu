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
        await StartBackgroundAsync(port);
    }

    public static async Task StartBackgroundAsync(int port)
    {
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        await client.ConnectAsync("localhost", port);
        await Console.Out.WriteLineAsync($"background connected server on " + port);

        var methods = new LocalMethods();

        _ = Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                var foregroundInfo = methods.GameIsForeground();
                if (!string.IsNullOrEmpty(foregroundInfo))
                    await CommonSendAsync(client, foregroundInfo);

                await Task.Delay(300);
            }
        });

        while (true)
        {
            await Task.Delay(3000);

            var musiceInfo = methods.GetMusicInfo();
            if (!string.IsNullOrEmpty(musiceInfo))
                await CommonSendAsync(client, musiceInfo);

            var gameInfo = methods.DetectGameRunning();
            if (!string.IsNullOrEmpty(gameInfo))
                await CommonSendAsync(client, gameInfo);
        }

    }

    public static async Task CommonSendAsync(Socket client, string data)
    {
        await client.SendAsync(Encoding.UTF8.GetBytes(data), SocketFlags.None);
    }
}
