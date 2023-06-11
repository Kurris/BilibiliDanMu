using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BDanMuLib.Converters;
using Newtonsoft.Json;

namespace BDanMuLib.Extensions
{
    internal static class StreamExtensions
    {
        internal static async Task<bool> ReadBAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellation = default)
        {
            if (offset + count > buffer.Length)
                throw new ArgumentException();

            var read = 0;
            while (read < count)
            {
                var available = await stream.ReadAsync(buffer.AsMemory(offset, count - read), cancellation);

                read += available;
                offset += available;

                if (available == 0)
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// 发送加入房间包
        /// </summary>
        /// <param name="RoomId">房间号</param>
        /// <param name="Token">凭证</param>
        /// <returns></returns>
        internal static async Task SendJoinRoomAsync(this Stream stream, int roomId, string token, CancellationToken cancellationToken = default)
        {
            var body = JsonConvert.SerializeObject(new
            {
                roomid = roomId,
                uid = 0,
                protover = 2,
                token,
                platform = "web"
            });

            await stream.SendSocketDataAsync(16, 7, 1, 2, body, cancellationToken);
        }



        /// <summary>
        /// 循环发送心跳包
        /// </summary>
        /// <returns></returns>
        internal static async Task SendHeartBeatLoopAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await stream.SendSocketDataAsync(16, 2, 1, 2, string.Empty, cancellationToken);
                //心跳只需要30秒激活一次,偏移检查
                await Task.Delay(30000, cancellationToken);
            }
        }



        internal static async Task SendSocketDataAsync(this Stream stream, short magic, int action, int param = 1, short ver = 2, string body = "", CancellationToken cancellationToken = default)
        {
            var playload = Encoding.UTF8.GetBytes(body);

            var buffer = new byte[playload.Length + 16];

            await using var ms = new MemoryStream(buffer);
            var b = EndianBitConverter.BigEndian.GetBytes(buffer.Length);

            await ms.WriteAsync(b.AsMemory(0, 4), cancellationToken);
            b = EndianBitConverter.BigEndian.GetBytes(magic);
            await ms.WriteAsync(b.AsMemory(0, 2), cancellationToken);
            b = EndianBitConverter.BigEndian.GetBytes(ver);
            await ms.WriteAsync(b.AsMemory(0, 2), cancellationToken);
            b = EndianBitConverter.BigEndian.GetBytes(action);
            await ms.WriteAsync(b.AsMemory(0, 4), cancellationToken);
            b = EndianBitConverter.BigEndian.GetBytes(param);
            await ms.WriteAsync(b.AsMemory(0, 4), cancellationToken);

            if (playload.Length > 0)
            {
                await ms.WriteAsync(playload, cancellationToken);
            }

            await stream.WriteAsync(buffer, cancellationToken);
        }
    }
}
