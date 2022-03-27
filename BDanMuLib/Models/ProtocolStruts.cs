using System;
using BDanMuLib.Converters;
using BDanmuLib.Models;

namespace BDanMuLib.Models
{
    /// <summary>
    /// 弹幕协议
    /// </summary>
    internal struct ProtocolStruts
    {
        /// <summary>
        /// 消息总长度 (协议头 + 数据长度)
        /// </summary>
        internal int PacketLength { get; private set; }

        /// <summary>
        /// 消息头长度 (固定为16[sizeof(Protocol)])
        /// </summary>
        internal short HeaderLength { get; private set; }

        /// <summary>
        /// 消息版本号
        /// </summary>
        internal short Version { get; private set; }

        /// <summary>
        /// 操作码
        /// </summary>
        internal OperateType OperateType { get; private set; }

        /// <summary>
        /// 参数, 固定为1
        /// </summary>
        internal int Parameter { get; private set; }

        /// <summary>
        /// 解析字节流
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        internal static ProtocolStruts FromBuffer(byte[] buffer)
        {
            if (buffer.Length < 16) throw new ArgumentException();
            
            return new ProtocolStruts
            {
                PacketLength = EndianBitConverter.BigEndian.ToInt32(buffer, 0),
                HeaderLength = EndianBitConverter.BigEndian.ToInt16(buffer, 4),
                Version = EndianBitConverter.BigEndian.ToInt16(buffer, 6),
                OperateType = (OperateType)EndianBitConverter.BigEndian.ToInt32(buffer, 8),
                Parameter = EndianBitConverter.BigEndian.ToInt32(buffer, 12),
            };
        }
    }
}
