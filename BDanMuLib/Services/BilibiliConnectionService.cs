using System;
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
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;
using BDanMuLib.Interfaces;
using Microsoft.Extensions.Logging;
using BDanMuLib.Enums;

namespace BDanMuLib.Services
{
    /// <summary>
    /// B站弹幕库核心
    /// </summary>
    internal class BilibiliConnectionService : IBarrageConnectionProvider
    {

        private Stream _stream;
        private readonly BufferReadState _state = new();
        private int? _roomId;

        private readonly BilibiliApiService _bilibiliApiService;
        private readonly RawtHandleService _rawtHandleService;
        private readonly ILogger<BilibiliConnectionService> _logger;



        public BilibiliConnectionService(ILogger<BilibiliConnectionService> logger, BilibiliApiService bilibiliApiService, RawtHandleService rawtHandleService)
        {
            _bilibiliApiService = bilibiliApiService;
            _rawtHandleService = rawtHandleService;
            _logger = logger;
        }


        /// <summary>
        /// 连接直播弹幕服务器
        /// </summary>
        /// <param name="roomId">房间号(可以为短号)</param>
        /// <param name="onReceive">消息处理方法</param>
        /// <returns></returns>
        /// <exception cref="SocketException"></exception>
        public async Task<bool> ConnectAsync(int roomId, Action<Result> onReceive, CancellationToken cancellation = default)
        {
            if (_stream != null || _roomId.HasValue)
            {
                throw new Exception("ConnectAsync method only can be called one time in instance.");
            }

            _roomId = roomId;


            //房间,直播ip和port信息
            var roomInfo = await _bilibiliApiService.GetRoomInfoAsync(roomId);
            if (roomInfo.LiveStatus != LiveStatusType.直播中)
            {
                _logger.LogInformation("RoomId:{RoomId} 's live status {Status} , connect operation finished", _roomId, roomInfo.LiveStatus.ToString());
                return false;
            }
            var broadCastInfo = await _bilibiliApiService.GetBroadCastInfoAsync(roomInfo.RoomId);

            try
            {
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

                _logger.LogInformation("Connect room:{roomId} successfully", roomId);

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
                _logger.LogInformation("Room:{RoomId} connection canceled by manually.", roomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);
            }

            return true;
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
                await _stream.ReadBAsync(stableBuffer, 0, length, _state, cancellation);

                ProtocolStruts protocol = ProtocolStruts.FromBuffer(stableBuffer);
                if (protocol.PacketLength < 16)
                {
                    continue;//throw new NotSupportedException("协议失败: (L:" + protocol.PacketLength + ")");
                }

                var payloadLength = protocol.PacketLength - 16;
                if (payloadLength == 0)
                {
                    //没有内容
                    continue;
                }

                var buffer = new byte[payloadLength];

                await _stream.ReadBAsync(buffer, 0, payloadLength, _state, cancellation);

                if (protocol.Version == 2 && protocol.OperateType == OperateType.DetailCommand)
                {
                    // 处理deflate消息 ,跳过0x78 0xDA
                    await using var ms = new MemoryStream(buffer, 2, payloadLength - 2);
                    await using var deflate = new DeflateStream(ms, CompressionMode.Decompress);
                    var headerBuffer = new byte[length];

                    while (!cancellation.IsCancellationRequested)
                    {
                        if (!await deflate.ReadBAsync(headerBuffer, 0, length, _state, cancellation))
                        {
                            break;
                        }
                        var protocolInfo = ProtocolStruts.FromBuffer(headerBuffer);
                        payloadLength = protocolInfo.PacketLength - length;

                        var danMuKuBuffer = new byte[payloadLength];
                        if (!await deflate.ReadBAsync(danMuKuBuffer, 0, payloadLength, _state, cancellation))
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
                            _logger.LogError(ex, ex.GetBaseException().Message);
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
                MessageType.DANMU_MSG => await _rawtHandleService.FromDanMuMsgAsync(jObj),
                MessageType.SEND_GIFT => _rawtHandleService.FromSendGift(jObj),
                MessageType.ENTRY_EFFECT => _rawtHandleService.FromEntryEffect(jObj),
                MessageType.SUPER_CHAT_MESSAGE => _rawtHandleService.FromSuperChat(jObj),
                MessageType.INTERACT_WORD => _rawtHandleService.FromInteractWord(jObj),
                MessageType.WATCHED_CHANGE => _rawtHandleService.FromWatchedChanged(jObj),
                MessageType.GUARD_BUY => _rawtHandleService.FromGuardBuy(jObj),
                MessageType.USER_TOAST_MSG => _rawtHandleService.FromUserToastMsg(jObj),
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
                while (_state.IsReading)
                {
                    await Task.Delay(10);
                }

                if (_stream != null)
                {
                    await _stream.DisposeAsync();
                    _stream = null;
                }
            }
            catch
            {

            }
        }

        public async ValueTask DisposeAsync()
        {
            _logger.LogInformation("{RoomId} receive raise Dispose", _roomId);
            await DisconnectAsync();
        }
    }
}
