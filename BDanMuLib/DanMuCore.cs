using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BDanMuLib.Converters;
using BDanmuLib.Models;
using BDanMuLib.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using BDanMuLib.Extensions;
using System.Net.Http;

namespace BDanMuLib
{
    /// <summary>
    /// 委托接收消息事件
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="obj"></param>
    public delegate void ReceiveMessage(MessageType messageType, object obj);


    /// <summary>
    /// 弹幕库核心
    /// </summary>
    public class DanMuCore
    {

        static DanMuCore()
        {
            getEmotes();
        }


        // /// <summary>
        // /// 直播弹幕地址
        // /// </summary>
        // private string[] _defaultHosts = { "livecmt-2.bilibili.com", "livecmt-1.bilibili.com" };

        /// <summary>
        /// 直播服务地址DNS
        /// </summary>
        private string _chatHost = "chat.bilibili.com";

        /// <summary>
        /// TCP端口
        /// </summary>
        private int _chatPort = 2243;

        /// <summary>
        /// Http客户端
        /// </summary>
        private static HttpClient Client = new();

        /// <summary>
        /// TCP客户端
        /// </summary>
        private TcpClient _tcpClient;

        /// <summary>
        /// 网络流
        /// </summary>
        private Stream _netStream;

        /// <summary>
        /// 是否已经连接
        /// </summary>
        private bool _isConnected;

        /// <summary>
        /// 接受消息
        /// </summary>
        public event ReceiveMessage ReceiveMessage;

        /// <summary>
        /// 协议版本
        /// </summary>
        private const short ProtocolVersion = 2;


        /// <summary>
        /// 前端数据唯一性
        /// </summary>
        private static Guid Key
        {
            get
            {
                return Guid.NewGuid();
            }
        }


        /// <summary>
        /// 头像
        /// </summary>
        private static Dictionary<string, string> _emotes;


        private static void getEmotes()
        {

            var text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "emote.json"), Encoding.UTF8);
            _emotes = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
        }


        /// <summary>
        /// 连接直播弹幕服务器
        /// </summary>
        /// <param name="roomId">房间号</param>
        /// <returns>连接结果</returns>
        public async Task<bool> ConnectAsync(int roomId)
        {
            try
            {
                if (_isConnected) throw new InvalidOperationException();

                string token;
                try
                {
                    //请求的内容
                    var requestContent = await Client.GetStringAsync(ApiUrls.BroadCastUrl + roomId);

                    var dataJToken = JObject.Parse(requestContent)["data"];

                    token = dataJToken["token"].Value<string>();
                    _chatHost = dataJToken["host"].Value<string>();
                    _chatPort = dataJToken["port"].Value<int>();

                }
                catch (Exception)
                {
                    return false;
                }

                _tcpClient = new TcpClient();

                var ipAddress = await Dns.GetHostAddressesAsync(_chatHost);

                var index = new Random().Next(ipAddress.Length);

                if (_tcpClient.Connected)
                {
                    throw new SocketException((int)SocketError.SocketError);
                }
                await _tcpClient.ConnectAsync(ipAddress[index], _chatPort);

                //同步流
                _netStream = Stream.Synchronized(_tcpClient.GetStream());

                if (!await SendJoinRoom(roomId, token)) return false;

                _isConnected = true;
                _ = HeartBeatLoop();
                _ = ReceiveMessageLoop();

                Console.WriteLine("连接房间号:" + roomId);
                return true;
            }
            catch (Exception)
            {
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
            var packageModel = new
            {
                roomid = RoomId,
                uid = 0,
                protover = ProtocolVersion,
                token = Token,
                platform = "web"
            };

            var body = JsonConvert.SerializeObject(packageModel);

            await SendSocketDataAsync(0, 16, ProtocolVersion, 7, 1, body);
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
                while (this._isConnected)
                {
                    await SendSocketDataAsync(0, 16, ProtocolVersion, 2, 1, string.Empty);
                    //心跳只需要30秒激活一次,偏移检查
                    await Task.Delay(30000);
                }
            }
            catch (Exception)
            {
                Disconnect();
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
                while (this._isConnected)
                {
                    await _netStream.ReadBAsync(stableBuffer, 0, 16);

                    var protocol = ProtocolStruts.FromBuffer(stableBuffer);
                    if (protocol.PacketLength < 16)
                    {
                        throw new NotSupportedException("协议失败: (L:" + protocol.PacketLength + ")");
                    }
                    var payLoadLength = protocol.PacketLength - 16;
                    if (payLoadLength == 0)
                    {
                        continue; // 没有内容了
                    }

                    var buffer = new byte[payLoadLength];

                    await _netStream.ReadBAsync(buffer, 0, payLoadLength);
                    if (protocol.Version == 2 && protocol.OperateType == OperateType.DetailCommand) // 处理deflate消息
                    {
                        await using (var ms = new MemoryStream(buffer, 2, payLoadLength - 2)) // Skip 0x78 0xDA
                        await using (var deflate = new DeflateStream(ms, CompressionMode.Decompress))
                        {
                            var headerBuffer = new byte[16];
                            try
                            {
                                while (true)
                                {
                                    if (!await deflate.ReadBAsync(headerBuffer, 0, 16))
                                    {
                                        break;
                                    }
                                    var protocolInfo = ProtocolStruts.FromBuffer(headerBuffer);
                                    payLoadLength = protocolInfo.PacketLength - 16;
                                    var danMuKuBuffer = new byte[payLoadLength];
                                    if (!await deflate.ReadBAsync(danMuKuBuffer, 0, payLoadLength))
                                    {
                                        break;
                                    }
                                    await HandleMsg(protocol.OperateType, danMuKuBuffer);
                                }
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    }
                    else
                    {
                        await HandleMsg(protocol.OperateType, buffer);
                    }
                }
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        /// <summary>
        /// 处理消息,具体的类型处理
        /// </summary>
        /// <param name="type">操作码</param>
        /// <param name="buffer">字节流</param>
        private async Task HandleMsg(OperateType type, byte[] buffer)
        {

            switch (type)
            {
                case OperateType.SendHeartBeat:
                    break;
                case OperateType.Hot:
                    var hot = EndianBitConverter.BigEndian.ToUInt32(buffer, 0);
                    ReceiveMessage?.Invoke(MessageType.NONE, hot);
                    break;
                case OperateType.DetailCommand:

                    var json = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                    string cmd;
                    try
                    {

                        var jObj = JObject.Parse(json);
                        cmd = jObj.Value<string>("cmd");


                        var cmdCommand = (MessageType)Enum.Parse(typeof(MessageType), cmd);

                        switch (cmdCommand)
                        {
                            case MessageType.DANMU_MSG:
                                {
                                    var info = jObj["info"];

                                    var mid = info[2][0].Value<string>();
                                    var isAdmin = info[2][2].Value<bool>();
                                    var time = info[0][4].Value<string>().ConvertStringToDateTime();
                                    var userName = info[2][1].Value<string>();
                                    var audRank = info[4][4].Value<int>();
                                    var comment = info[1].Value<string>();
                                    var top3 = info[4][4].Value<int>();
                                    var color = info[2][7].Value<string>();
                                    //#e5f1f9

                                    foreach (var item in _emotes)
                                    {
                                        string emote = "[" + item.Key + "]";
                                        if (comment.Contains(emote))
                                        {
                                            comment = comment.Replace(emote, "<img referrer=\"no-referrer\" height=\"20\" width=\"20\" src=\"" + item.Value + "\"/>");
                                        }
                                    }

                                    var medal = info[3];
                                    var hasMedal = medal.Any();
                                    string medalName = null;
                                    string level = null;
                                    if (hasMedal)
                                    {
                                        medalName = medal[1].Value<string>();
                                        level = medal[0].Value<string>();
                                    }

                                    string faceUrl = null;
                                    try
                                    {
                                        string response = await Client.GetStringAsync($"https://space.bilibili.com/{mid}");
                                        int i = response.IndexOf("href=\"//i0.hdslb.com");
                                        if (i == -1)
                                        {
                                            i = response.IndexOf("href=\"//i1.hdslb.com");
                                            if (i == -1)
                                            {
                                                i = response.IndexOf("href=\"//i2.hdslb.com");
                                            }
                                        }
                                        if (i == -1)
                                        {

                                        }
                                        else
                                        {
                                            response = response.Substring(i);
                                            faceUrl = string.Concat("http:", response.AsSpan(6, response.IndexOf(".jpg\">") - 2));
                                        }
                                    }
                                    catch (Exception)
                                    {

                                    }


                                    ReceiveMessage?.Invoke(MessageType.DANMU_MSG, new
                                    {
                                        mid,
                                        faceUrl,
                                        comment,
                                        isAdmin,
                                        time,
                                        userName,
                                        audRank,
                                        hasMedal,
                                        medalName,
                                        level,
                                        top3,
                                        color,
                                        key = Key,
                                    });

                                }
                                break;
                            case MessageType.SEND_GIFT:
                                {
                                    var dataJToken = jObj["data"];
                                    var userName = dataJToken["uname"].Value<string>();
                                    var action = dataJToken["action"].Value<string>();
                                    var giftName = dataJToken["giftName"].Value<string>();
                                    var num = dataJToken["num"].Value<int>();

                                    ReceiveMessage?.Invoke(MessageType.SEND_GIFT, $"{userName}{action}{giftName} x {num}");
                                }
                                break;
                            case MessageType.WELCOME_GUARD:
                                break;
                            case MessageType.INTERACT_WORD:
                                {
                                    var dataToken = jObj["data"];
                                    var userName = dataToken["uname"].Value<string>();
                                    ReceiveMessage?.Invoke(cmdCommand, $"{userName} 进入直播间");
                                    break;
                                }
                            //case MessageType.ONLINE_RANK_COUNT:
                            //    {
                            //        var rank = jObj["data"]["count"].Value<string>();
                            //        ReceiveMessage?.Invoke(cmdCommand, rank);
                            //    break;
                            //    }

                            case MessageType.NOTICE_MSG:
                                break;
                            case MessageType.WATCHED_CHANGE:
                                {
                                    var watchedNum = jObj["data"]["num"].Value<string>();
                                    ReceiveMessage?.Invoke(MessageType.WATCHED_CHANGE, watchedNum);
                                }
                                break;
                            case MessageType.ROOM_REAL_TIME_MESSAGE_UPDATE:
                                break;
                            case MessageType.LIVE_INTERACTIVE_GAME:
                                {

                                }
                                break;
                            //case MessageType.HOT_RANK_CHANGED:
                            //    {
                            //        var rank = jObj["data"]["rank"].Value<string>();
                            //        ReceiveMessage?.Invoke(MessageType.HOT_RANK_CHANGED, rank);
                            //    }
                            //    break;
                            //case MessageType.HOT_ROOM_NOTIFY:
                            //    break;
                            //case MessageType.HOT_RANK_CHANGED_V2:
                            //    {
                            //        var rank = jObj["data"]["rank"].Value<string>();
                            //        ReceiveMessage?.Invoke(MessageType.HOT_RANK_CHANGED_V2, rank);
                            //    }
                            //    break;
                            case MessageType.ONLINE_RANK_TOP3:
                                {
                                    var list = jObj["data"]["list"].ToList();
                                    ReceiveMessage?.Invoke(cmdCommand, list);
                                }
                                break;
                            //case MessageType.ONLINE_RANK_V2:
                            //    {
                            //        var list = jObj["data"]["list"];
                            //    }
                            //    break;
                            //case MessageType.COMBO_SEND:
                            //    {
                            //        var action = jObj["data"]["action"].Value<string>();
                            //        var gift = jObj["data"]["gift_name"].Value<string>();
                            //        var sendUser = jObj["data"]["uname"].Value<string>();
                            //        var combo = jObj["data"]["combo_num"].Value<int>();
                            //    }
                            //    break;
                            case MessageType.ENTRY_EFFECT:
                                {
                                    var data = jObj["data"];
                                    ReceiveMessage?.Invoke(cmdCommand, data);
                                }
                                break;
                            case MessageType.LIKE_INFO_V3_UPDATE:
                                {

                                }
                                break;
                            case MessageType.GUARD_BUY:
                                {

                                }
                                break;
                            case MessageType.SUPER_CHAT_MESSAGE:
                                {
                                    ReceiveMessage?.Invoke(cmdCommand, jObj["data"].ToString());
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    break;
                case OperateType.AuthAndJoinRoom:
                    break;
                case OperateType.ReceiveHeartBeat:
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// 发送套字节数据
        /// </summary>
        /// <param name="packLength"></param>
        /// <param name="magic"></param>
        /// <param name="ver"></param>
        /// <param name="action"></param>
        /// <param name="param"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private async Task SendSocketDataAsync(int packLength, short magic, short ver, int action, int param = 1, string body = "")
        {
            var playLoad = Encoding.UTF8.GetBytes(body);
            if (packLength == 0)
            {
                packLength = playLoad.Length + 16;
            }
            var buffer = new byte[packLength];

            // ReSharper disable once ConvertToUsingDeclaration
            await using (var ms = new MemoryStream(buffer))
            {
                var b = EndianBitConverter.BigEndian.GetBytes(buffer.Length);

                await ms.WriteAsync(b, 0, 4);
                b = EndianBitConverter.BigEndian.GetBytes(magic);
                await ms.WriteAsync(b, 0, 2);
                b = EndianBitConverter.BigEndian.GetBytes(ver);
                await ms.WriteAsync(b, 0, 2);
                b = EndianBitConverter.BigEndian.GetBytes(action);
                await ms.WriteAsync(b, 0, 4);
                b = EndianBitConverter.BigEndian.GetBytes(param);
                await ms.WriteAsync(b, 0, 4);

                if (playLoad.Length > 0)
                {
                    await ms.WriteAsync(playLoad, 0, playLoad.Length);
                }

                await _netStream.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            if (!_isConnected) return;

            _isConnected = false;

            try
            {
                _tcpClient.Close();
                _tcpClient.Dispose();
                _tcpClient = null;

                _netStream.Dispose();
                _netStream = null;
            }
            catch (Exception)
            {
                // ignored
            }


        }
    }
}
