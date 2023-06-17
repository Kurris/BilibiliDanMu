using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json.Linq;

namespace LiveBackgroundService;
public static class Program
{
    public static async Task Main(string[] args)
    {
        await StartServerAsync(6000);
    }

    public static async Task StartServerAsync(int port)
    {
        var endpoint = new IPEndPoint(IPAddress.Any, port);
        var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(endpoint);
        listener.Listen(10);

        await Console.Out.WriteLineAsync($"Socket binding {endpoint.Address}:{endpoint.Port}");

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
                int length = await sender.ReceiveAsync(receiveBuffer, SocketFlags.None);
                data += Encoding.ASCII.GetString(receiveBuffer, 0, length);

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
                        await sender.SendAsync(Encoding.ASCII.GetBytes(result), SocketFlags.None);

                        if (longReceive)
                        {
                            await Task.Delay(1000 * 10);
                        }

                    } while (longReceive);
                }
            }
            catch (Exception)
            {
                await Console.Out.WriteLineAsync($"断开连接");
                break;
            }
        }

        sender.Shutdown(SocketShutdown.Both);
        sender.Close();
    }
}
