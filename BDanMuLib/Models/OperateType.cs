using System;
using System.Collections.Generic;
using System.Text;

namespace BDanmuLib.Models
{
    /// <summary>
    /// 操作码
    /// </summary>
    internal enum OperateType
    {
        SendHeartBeat = 2,
        Hot = 3,
        DetailCommand = 5,
        AuthAndJoinRoom = 7,
        ReceiveHeartBeat = 8
    }
}
