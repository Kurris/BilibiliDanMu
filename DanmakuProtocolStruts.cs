using BitConverter;
using System;

namespace BilibiliDanMuLib
{
    /// <summary>
    /// 弹幕协议
    /// </summary>
    public struct DanmakuProtocolStruts
    {

        /// <summary>
        /// 消息总长度 (协议头 + 数据长度)
        /// </summary>
        internal int PacketLength;

        /// <summary>
        /// 消息头长度 (固定为16[sizeof(DanmakuProtocol)])
        /// </summary>
        internal short HeaderLength;

        /// <summary>
        /// 消息版本号
        /// </summary>
        internal short Version;

        /// <summary>
        /// 操作码
        /// </summary>
        internal OperateCode OpearateCode;

        /// <summary>
        /// 参数, 固定为1
        /// </summary>
        internal int Parameter;

        /// <summary>
        /// 解析字节流
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        internal static DanmakuProtocolStruts FromBuffer(byte[] buffer)
        {
            if (buffer.Length < 16) { throw new ArgumentException(); }
            return new DanmakuProtocolStruts()
            {
                PacketLength = EndianBitConverter.BigEndian.ToInt32(buffer, 0),
                HeaderLength = EndianBitConverter.BigEndian.ToInt16(buffer, 4),
                Version = EndianBitConverter.BigEndian.ToInt16(buffer, 6),
                OpearateCode = (OperateCode)EndianBitConverter.BigEndian.ToInt32(buffer, 8),
                Parameter = EndianBitConverter.BigEndian.ToInt32(buffer, 12),
            };
        }
    }

    /// <summary>
    /// 操作码
    /// </summary>
    internal enum OperateCode
    {
        客户端发送的心跳包 = 2,
        人气值节整数 = 3,
        表示具体命令Cmd = 5,
        认证并加入房间 = 7,
        服务器发送的心跳包 = 8
    }

    /// <summary>
    /// 具体命令
    /// </summary>
    public enum Cmd
    {
        /// <summary>
        /// 弹幕信息
        /// </summary>
        DANMU_MSG,

        /// <summary>
        /// 礼物信息
        /// </summary>
        SEND_GIFT,

        /// <summary>
        /// 欢迎信息
        /// </summary>
        WELCOME,

        /// <summary>
        /// 欢迎房管
        /// </summary>
        WELCOME_GUARD,

        /// <summary>
        /// 系统信息
        /// </summary>
        SYS_MSG,

        /// <summary>
        /// 主播准备中
        /// </summary>
        PREPARING,

        /// <summary>
        /// 正在直播
        /// </summary>
        LIVE,

        /// <summary>
        /// 许愿瓶
        /// </summary>
        WISH_BOTTLE,

        /// <summary>
        /// 进入房间
        /// </summary>
        INTERACT_WORD,
    }
}
