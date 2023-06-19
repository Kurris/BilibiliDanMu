using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json.Linq;

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

        if (IsPortAvalaible(port))
        {
            await StartServerAsync(port);
        }
    }

    public static async Task StartServerAsync(int port)
    {
        var endpoint = new IPEndPoint(IPAddress.Any, port);
        var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(endpoint);
        listener.Listen(10);

        await Console.Out.WriteLineAsync($"Live backend binding {endpoint.Address}:{endpoint.Port}");

        while (true)
        {
            var sender = await listener.AcceptAsync();
            _ = ReceiveAsync(sender);
        }
    }


    public static async Task ReceiveAsync(Socket sender)
    {
        var receiveBuffer = new byte[256];
        string data = string.Empty;

        while (true)
        {
            try
            {
                //暂时不考虑流长度,持续读取的问题
                int length = await sender.ReceiveAsync(receiveBuffer, SocketFlags.None);
                data += Encoding.UTF8.GetString(receiveBuffer, 0, length);

                //终止连接
                if (data.StartsWith("<command>|end") && data.EndsWith("<eof>")) break;

                //执行特定方法
                if (data.StartsWith("<command>"))
                {
                    var strs = data.Split("|");
                    var methodName = strs[1];

                    var args = JObject.Parse(strs[3]).PropertyValues().Select(x => x.Value<object>()).ToArray();
                    var longReceive = Convert.ToBoolean(int.Parse(strs[5]));

                    await Console.Out.WriteLineAsync($"command:{methodName},args:{strs[3]},longReceive:{longReceive}");

                    var type = typeof(LocalMethods);
                    object obj = Activator.CreateInstance(type)!;

                    do
                    {
                        var result = type.GetMethod(methodName)!.Invoke(obj, args)!.ToString()!;
                        //await Console.Out.WriteLineAsync("result:" + result);
                        await sender.SendAsync(Encoding.UTF8.GetBytes(result), SocketFlags.None);

                    } while (longReceive);
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"break by :{ex.Message}");
                break;
            }
        }

        sender.Shutdown(SocketShutdown.Both);
        sender.Close();
    }



    private static bool IsPortAvalaible(int myPort)
    {
        var usedPorts = new List<int>();
        var properties = IPGlobalProperties.GetIPGlobalProperties();

        // Active tcp listners
        var endPointsTcp = properties.GetActiveTcpListeners().Select(x => x.Port);
        usedPorts.AddRange(endPointsTcp);

        // Active udp listeners
        var endPointsUdp = properties.GetActiveUdpListeners().Select(x => x.Port);
        usedPorts.AddRange(endPointsUdp);

        return !usedPorts.Contains(myPort);
    }
}
