using System.Net.Sockets;
using System.Text;

namespace LiveBackgroundService;

/// <summary>
/// socket client
/// </summary>
public static class Program
{
    /// <summary>
    /// args: port,detect game , detect music
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static async Task Main(string[] args)
    {
        var port = int.Parse(args[0]);
        var detectGame = bool.Parse(args[1]);
        var detectMusice = bool.Parse(args[2]);

        await Console.Out.WriteLineAsync($"port:{port},detectGame:{detectGame},detectMusice:{detectMusice}");

        var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        await client.ConnectAsync("localhost", port);
        await Console.Out.WriteLineAsync($"background connected server on " + port);
        await StartBackgroundAsync(client, detectGame, detectMusice);
    }

    public static async Task StartBackgroundAsync(Socket client, bool detectGame, bool detectMusice)
    {
        var methods = new LocalMethods();

        if (detectGame)
        {
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
        }


        while (true)
        {
            await Task.Delay(3000);

            if (detectMusice)
            {
                var musiceInfo = methods.GetMusicInfo();
                if (!string.IsNullOrEmpty(musiceInfo))
                    await CommonSendAsync(client, musiceInfo);
            }


            if (detectGame)
            {
                var gameInfo = methods.DetectGameRunning();
                if (!string.IsNullOrEmpty(gameInfo))
                    await CommonSendAsync(client, gameInfo);
            }
        }
    }

    public static async Task CommonSendAsync(Socket client, string data)
    {
        //await Console.Out.WriteLineAsync(data);
        await client.SendAsync(Encoding.UTF8.GetBytes(data), SocketFlags.None);
    }
}
