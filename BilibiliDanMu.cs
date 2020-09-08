using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BitConverter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BilibiliDanMuLib
{
    /// <summary>
    /// 破站弹幕库
    /// </summary>
    public class BilibiliDanMu
    {
        /// <summary>
        /// 用户直播地址
        /// </summary>
        private readonly string _mBroadcastUrl = "https://api.live.bilibili.com/room/v1/Danmu/getConf?room_id=";

        /// <summary>
        /// 直播弹幕地址
        /// </summary>
        private string[] _mDefaultHosts = { "livecmt-2.bilibili.com", "livecmt-1.bilibili.com" };

        /// <summary>
        /// 直播服务地址DNS
        /// </summary>
        private string _mChatHost = "chat.bilibili.com";

        /// <summary>
        /// TCP端口
        /// </summary>
        private int _mChatPort = 2243;

        /// <summary>
        /// Http客户端
        /// </summary>
        private static HttpClient _mHttpClient = null;

        /// <summary>
        /// TCP客户端
        /// </summary>
        private TcpClient _mTcpClient = null;

        /// <summary>
        /// 网络流
        /// </summary>
        private Stream _mNetStream;

        /// <summary>
        /// 是否已经连接
        /// </summary>
        private bool _mConnected = false;

        /// <summary>
        /// 日志事件
        /// </summary>
        public event InfoLogMsg Log;

        /// <summary>
        /// 消息输出
        /// </summary>
        public event MsgOutPut OutPut;

        /// <summary>
        /// 协议版本
        /// </summary>
        private short _mProtocolVer = 2;

        /// <summary>
        /// 最后使用的房间号
        /// </summary>
        private static int _mLastRoomid;

        /// <summary>
        /// 最后使用的服务地址
        /// </summary>
        private static string _mLastSrv;

        /// <summary>
        /// 连接直播弹幕服务器
        /// </summary>
        /// <param name="RoomId">房间号</param>
        /// <returns>连接结果</returns>
        public async Task<bool> ConnectAsync(int RoomId)
        {
            try
            {
                if (this._mConnected) throw new InvalidOperationException();

                if (RoomId == _mLastRoomid) _mChatHost = _mLastSrv;

                string sToken = string.Empty;
                try
                {
                    if (_mHttpClient == null)
                    {
                        _mHttpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) };
                    }
                    //请求的内容
                    var sRequestContent = await _mHttpClient.GetStringAsync(_mBroadcastUrl + RoomId);

                    var DataJToken = JObject.Parse(sRequestContent)["data"];

                    sToken = DataJToken["token"] + "";
                    _mChatHost = DataJToken["host"] + "";
                    _mChatPort = DataJToken["port"].Value<int>();

                }
                catch (Exception ex)
                {
                    Log?.Invoke(ex.StackTrace);
                }

                _mTcpClient = new TcpClient();

                var ipAddress = await Dns.GetHostAddressesAsync(_mChatHost);

                var iIndex = new Random().Next(ipAddress.Length);

                if (_mTcpClient.Connected)
                {
                    throw new SocketException((int)SocketError.SocketError);
                }
                await _mTcpClient.ConnectAsync(ipAddress[iIndex], _mChatPort);

                //同步流
                _mNetStream = Stream.Synchronized(_mTcpClient.GetStream());

                if (await SendJoinRoom(RoomId, sToken))
                {
                    _mConnected = true;
                    _ = HeartBeatLoop();
                    _ = ReceiveMessageLoop();
                    _mLastSrv = _mChatHost;
                    _mLastRoomid = RoomId;

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log?.Invoke(ex.StackTrace);

                return false;
            }
        }

        /// <summary>
        /// 发送加入房间包
        /// </summary>
        /// <param name="RoomId">房间号</param>
        /// <param name="Token">凭证</param>
        /// <returns></returns>
        private async Task<bool> SendJoinRoom(int RoomId, string Token)
        {
            var PackageModel = new
            {
                roomid = RoomId,
                uid = 0,
                protover = _mProtocolVer,
                token = Token,
                platform = "web"
            };

            var sJson = JsonConvert.SerializeObject(PackageModel);

            await SendSocketDataAsync(0, 16, _mProtocolVer, 7, 1, sJson);

            return true;
        }

        /// <summary>
        /// 循环发送心跳包
        /// </summary>
        /// <returns></returns>
        private async Task HeartBeatLoop()
        {
            try
            {
                while (this._mConnected)
                {
                    await this.SendHeartbeatAsync();
                    //延迟30秒
                    await Task.Delay(30000);
                }
            }
            catch (Exception ex)
            {
                Disconnect();
                Log?.Invoke(ex.StackTrace);
            }
        }

        /// <summary>
        /// 循环接受信息
        /// </summary>
        /// <returns></returns>
        private async Task ReceiveMessageLoop()
        {
            try
            {
                var stableBuffer = new byte[16];
                var buffer = new byte[4096];
                while (this._mConnected)
                {
                    await _mNetStream.ReadBAsync(stableBuffer, 0, 16);

                    var protocol = DanmakuProtocolStruts.FromBuffer(stableBuffer);
                    if (protocol.PacketLength < 16)
                    {
                        throw new NotSupportedException("协议失败: (L:" + protocol.PacketLength + ")");
                    }
                    var payloadlength = protocol.PacketLength - 16;
                    if (payloadlength == 0)
                    {
                        continue; // 没有内容了
                    }

                    buffer = new byte[payloadlength];

                    await _mNetStream.ReadBAsync(buffer, 0, payloadlength);
                    if (protocol.Version == 2 && protocol.OpearateCode == OperateCode.表示具体命令Cmd) // 处理deflate消息
                    {
                        using (var ms = new MemoryStream(buffer, 2, payloadlength - 2)) // Skip 0x78 0xDA
                        using (var deflate = new DeflateStream(ms, CompressionMode.Decompress))
                        {
                            var headerbuffer = new byte[16];
                            try
                            {
                                while (true)
                                {
                                    await deflate.ReadBAsync(headerbuffer, 0, 16);
                                    var protocol_in = DanmakuProtocolStruts.FromBuffer(headerbuffer);
                                    payloadlength = protocol_in.PacketLength - 16;
                                    var danmakubuffer = new byte[payloadlength];
                                    await deflate.ReadBAsync(danmakubuffer, 0, payloadlength);
                                    HandleMsg(protocol.OpearateCode, danmakubuffer);
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                    else
                    {
                        HandleMsg(protocol.OpearateCode, buffer);
                    }
                }
            }
            catch (Exception ex)
            {
                Disconnect();
                Log?.Invoke(ex.StackTrace);
            }
        }

        /// <summary>
        /// 处理消息,具体的类型处理
        /// </summary>
        /// <param name="Code">操作码</param>
        /// <param name="buffer">字节流</param>
        private void HandleMsg(OperateCode Code, byte[] buffer)
        {

            switch (Code)
            {
                case OperateCode.客户端发送的心跳包:
                    break;
                case OperateCode.人气值节整数:

                    var Viewer = EndianBitConverter.BigEndian.ToUInt32(buffer, 0);
                    OutPut?.Invoke(Cmd.NONE, $"直播间人气值:{Viewer.ToString()}");

                    break;
                case OperateCode.表示具体命令Cmd:

                    string sJson = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                    try
                    {
                        var Jobj = JObject.Parse(sJson);

                        var Cmd = Jobj.Value<string>("cmd");

                        var CmdCommand = (Cmd)Enum.Parse(typeof(Cmd), Cmd);

                        switch (CmdCommand)
                        {
                            case BilibiliDanMuLib.Cmd.DANMU_MSG:
                                {
                                    var Infos = Jobj["info"];
                                    //Console.WriteLine($"{token[2][1]}:{token[1]}");
                                    OutPut?.Invoke(BilibiliDanMuLib.Cmd.DANMU_MSG, $"{Infos[2][1]}:{Infos[1]}");
                                }
                                break;
                            case BilibiliDanMuLib.Cmd.SEND_GIFT:
                                {
                                    OutPut?.Invoke(BilibiliDanMuLib.Cmd.SEND_GIFT, sJson);
                                }
                                break;
                            case BilibiliDanMuLib.Cmd.WELCOME:
                                break;
                            case BilibiliDanMuLib.Cmd.WELCOME_GUARD:
                                break;
                            case BilibiliDanMuLib.Cmd.SYS_MSG:
                                break;
                            case BilibiliDanMuLib.Cmd.PREPARING:
                                break;
                            case BilibiliDanMuLib.Cmd.LIVE:
                                break;
                            case BilibiliDanMuLib.Cmd.WISH_BOTTLE:
                                break;
                            case BilibiliDanMuLib.Cmd.INTERACT_WORD:
                                {
                                    var DataToken = Jobj["data"];
                                    string sUserName = DataToken["uname"].Value<string>();
                                    OutPut?.Invoke(BilibiliDanMuLib.Cmd.INTERACT_WORD, $"{sUserName}进入直播间");
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    catch (Exception)
                    {

                    }

                    break;
                case OperateCode.认证并加入房间:
                    break;
                case OperateCode.服务器发送的心跳包:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 发送心跳包
        /// </summary>
        /// <returns></returns>
        private async Task SendHeartbeatAsync()
        {
            await SendSocketDataAsync(0, 16, _mProtocolVer, 2, 1, string.Empty);
        }

        /// <summary>
        /// 发送套字节数据
        /// </summary>
        /// <param name="PackLength"></param>
        /// <param name="Magic"></param>
        /// <param name="Ver"></param>
        /// <param name="Action"></param>
        /// <param name="Param"></param>
        /// <param name="Body"></param>
        /// <returns></returns>
        async Task SendSocketDataAsync(int PackLength, short Magic, short Ver, int Action, int Param = 1, string Body = "")
        {
            var playload = Encoding.UTF8.GetBytes(Body);
            if (PackLength == 0)
            {
                PackLength = playload.Length + 16;
            }
            var buffer = new byte[PackLength];
            using (var ms = new MemoryStream(buffer))
            {
                var b = EndianBitConverter.BigEndian.GetBytes(buffer.Length);

                await ms.WriteAsync(b, 0, 4);
                b = EndianBitConverter.BigEndian.GetBytes(Magic);
                await ms.WriteAsync(b, 0, 2);
                b = EndianBitConverter.BigEndian.GetBytes(Ver);
                await ms.WriteAsync(b, 0, 2);
                b = EndianBitConverter.BigEndian.GetBytes(Action);
                await ms.WriteAsync(b, 0, 4);
                b = EndianBitConverter.BigEndian.GetBytes(Param);
                await ms.WriteAsync(b, 0, 4);

                if (playload.Length > 0)
                {
                    await ms.WriteAsync(playload, 0, playload.Length);
                }

                await _mNetStream.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            if (!_mConnected) return;

            _mConnected = false;

            try
            {
                _mTcpClient.Close();
            }
            catch (Exception ex)
            {
                Log?.Invoke(ex.StackTrace);
            }

            _mNetStream = null;
        }
    }
}