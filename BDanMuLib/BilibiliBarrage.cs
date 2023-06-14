﻿using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BDanMuLib.Converters;
using BDanmuLib.Enums;
using BDanMuLib.Models;
using Newtonsoft.Json.Linq;
using BDanMuLib.Extensions;
using BDanMuLib.Utils;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;
using BDanMuLib.Interfaces;

namespace BDanMuLib
{
    /// <summary>
    /// B站弹幕库核心
    /// </summary>
    public class BilibiliBarrage : IBarrageProvider
    {
        /// <summary>
        /// 网络流
        /// </summary>
        private Stream _stream;


        /// <summary>
        /// 连接直播弹幕服务器
        /// </summary>
        /// <param name="roomId">房间号(可以为短号)</param>
        /// <param name="onReceive">消息处理方法</param>
        /// <returns></returns>
        /// <exception cref="SocketException"></exception>
        public async Task ConnectAsync(int roomId, Action<Result> onReceive, CancellationToken cancellation = default)
        {
            if (_stream != null) await DisconnectAsync();
            cancellation.Register(async () =>
            {
                await DisconnectAsync();
            });

            //房间,直播ip和port信息
            var roomInfo = await RequestUtils.GetRoomInfoAsync(roomId);
            var broadCastInfo = await RequestUtils.GetBroadCastInfoAsync(roomInfo.RoomId);
            var ipAddresses = await Dns.GetHostAddressesAsync(broadCastInfo.Host, cancellation);

            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(broadCastInfo.Host, broadCastInfo.Port, cancellation);
            if (!tcpClient.Connected)
            {
                throw new SocketException((int)SocketError.ConnectionRefused);
            }

            _stream = Stream.Synchronized(tcpClient.GetStream());

            await _stream.SendJoinRoomAsync(roomInfo.RoomId, broadCastInfo.Token, cancellation);
            _ = _stream.SendHeartBeatLoopAsync(cancellation);

            await Console.Out.WriteLineAsync($"Connect room:{roomId} successfully");

            try
            {
                await ReceiveRawMessageLoopAsync(cancellation).ForEachAwaitWithCancellationAsync(async (x, _) =>
                {
                    var result = await HandleRawMessageAsync(x);

                    if (result.Type != MessageType.NONE)
                    {
                        onReceive.Invoke(result);
                    }
                }, cancellation);
            }
            catch (TaskCanceledException)
            {
                await Console.Out.WriteLineAsync($"Task Canceled");
            }
        }


        /// <summary>
        /// 接受stream数据
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        private async IAsyncEnumerable<MessageTypeWithRawDetail> ReceiveRawMessageLoopAsync([EnumeratorCancellation] CancellationToken cancellation = default)
        {
            var stableBuffer = new byte[16];
            var length = 16;
            while (!cancellation.IsCancellationRequested)
            {
                await _stream.ReadBAsync(stableBuffer, 0, length, cancellation);

                ProtocolStruts protocol = ProtocolStruts.FromBuffer(stableBuffer);
                if (protocol.PacketLength < 16)
                {
                    throw new NotSupportedException("协议失败: (L:" + protocol.PacketLength + ")");
                }

                var payloadLength = protocol.PacketLength - 16;
                if (payloadLength == 0)
                {
                    //没有内容
                    continue;
                }

                var buffer = new byte[payloadLength];

                await _stream.ReadBAsync(buffer, 0, payloadLength, cancellation);

                if (protocol.Version == 2 && protocol.OperateType == OperateType.DetailCommand)
                {
                    // 处理deflate消息 ,跳过0x78 0xDA
                    await using var ms = new MemoryStream(buffer, 2, payloadLength - 2);
                    await using var deflate = new DeflateStream(ms, CompressionMode.Decompress);
                    var headerBuffer = new byte[length];

                    while (!cancellation.IsCancellationRequested)
                    {
                        if (!await deflate.ReadBAsync(headerBuffer, 0, length, cancellation))
                        {
                            break;
                        }
                        var protocolInfo = ProtocolStruts.FromBuffer(headerBuffer);
                        payloadLength = protocolInfo.PacketLength - length;

                        var danMuKuBuffer = new byte[payloadLength];
                        if (!await deflate.ReadBAsync(danMuKuBuffer, 0, payloadLength, cancellation))
                        {
                            break;
                        }

                        var json = Encoding.UTF8.GetString(danMuKuBuffer, 0, danMuKuBuffer.Length);
                        var jObj = JObject.Parse(json);
                        string cmd = jObj.Value<string>("cmd");
                        MessageType type;

                        try
                        {
                            type = (MessageType)Enum.Parse(typeof(MessageType), cmd);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            continue;
                        }

                        yield return new MessageTypeWithRawDetail()
                        {
                            Type = type,
                            Raw = json
                        };
                    }
                }
                else
                {
                    if (protocol.OperateType == OperateType.Hot)
                    {
                        var hot = EndianBitConverter.BigEndian.ToUInt32(buffer, 0);
                        yield return new MessageTypeWithRawDetail()
                        {
                            Type = MessageType.HOT,
                            Raw = hot.ToString()
                        };
                    }
                }
            }
        }


        /// <summary>
        /// 处理原数据
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private async Task<Result> HandleRawMessageAsync(MessageTypeWithRawDetail info)
        {
            if (info.Type == MessageType.HOT)
            {
                return new Result(info.Type, new HotInfo(info.Raw));
            }

            var jObj = JObject.Parse(info.Raw);

            return info.Type switch
            {
                MessageType.DANMU_MSG => new Result(info.Type, await jObj.FromDanMuMsgAsync()),
                MessageType.SEND_GIFT => new Result(info.Type, jObj.FromSendGift()),
                MessageType.ENTRY_EFFECT => new Result(info.Type, jObj.FromEntryEffect()),
                MessageType.SUPER_CHAT_MESSAGE => new Result(info.Type, jObj.FromSuperChat()),
                MessageType.INTERACT_WORD => new Result(info.Type, jObj.FromInteractWord()),
                MessageType.WATCHED_CHANGE => new Result(info.Type, jObj.FromWatchedChanged()),
                MessageType.GUARD_BUY => new Result(info.Type, jObj.FromWatchedChanged()),
                MessageType.USER_TOAST_MSG => new Result(info.Type, jObj.FromUserToastMsg()),
                _ => Result.Default
            };
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public async ValueTask DisconnectAsync()
        {
            try
            {
                if (_stream != null)
                {
                    _stream.Close();
                    await _stream.DisposeAsync();
                    _stream = null;
                }
            }
            catch
            {

            }
        }

    }
}