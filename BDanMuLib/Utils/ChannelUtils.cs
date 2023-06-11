using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using BDanMuLib.Extensions;

namespace BDanMuLib.Utils
{
    public class ChannelUtils
    {
        public static async Task PublishAsync(string message)
        {
            await _boundedChannel.Value.Writer.WriteAsync(message);
        }


        /// <summary>
        /// 通过懒加载创建有限容量通道
        /// </summary>
        /// <remarks>默认容量为 1000</remarks>
        private static readonly Lazy<Channel<string>> _boundedChannel = new(() =>
        {
            //20够sc用了吧...
            var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(20)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = true
            });

            StartReader(channel);
            return channel;
        });


        /// <summary>                                                                                                                
        /// 创建一个读取器
        /// </summary>
        /// <param name="channel"></param>
        private static void StartReader(Channel<string, string> channel)
        {
            var reader = channel.Reader;

            // 创建长时间线程管道读取器
            _ = Task.Factory.StartNew(async () =>
            {
                while (await reader.WaitToReadAsync())
                {
                    if (!reader.TryRead(out var message)) continue;
                    message.Speak();
                }
            }, TaskCreationOptions.LongRunning); //LongRunning单独一个线程处理Channel任务
        }
    }
}
