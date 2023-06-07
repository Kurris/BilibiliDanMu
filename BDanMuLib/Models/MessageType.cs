using System;
using System.Collections.Generic;
using System.Text;

namespace BDanmuLib.Models
{
    /// <summary>
    /// 具体命令
    /// </summary>
    public enum MessageType
    {

        /// <summary>
        /// 无命令
        /// </summary>
        NONE,

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

        /// <summary>
        /// 榜单排名数
        /// </summary>
        ONLINE_RANK_COUNT,

        /// <summary>
        /// 消息通知
        /// </summary>
        NOTICE_MSG,

        /// <summary>
        /// 暂时无用
        /// </summary>
        STOP_LIVE_ROOM_LIST,

        /// <summary>
        /// 观看次数
        /// </summary>
        WATCHED_CHANGE,

        /// <summary>
        /// 房间即时信息更新
        /// </summary>
        ROOM_REAL_TIME_MESSAGE_UPDATE,

        /// <summary>
        /// 实时交互游戏
        /// </summary>
        LIVE_INTERACTIVE_GAME,

        /// <summary>
        /// 热度排名
        /// </summary>
        HOT_RANK_CHANGED,


        /// <summary>
        /// 热度房间推荐
        /// </summary>
        HOT_ROOM_NOTIFY,

        WIDGET_BANNER,

        /// <summary>
        /// 全站实时排名
        /// </summary>
        HOT_RANK_CHANGED_V2,

        /// <summary>
        /// 榜单前三更新(貌似不会触发了)
        /// </summary>
        ONLINE_RANK_TOP3,

        /// <summary>
        /// 榜单排名
        /// </summary>
        ONLINE_RANK_V2,

        /// <summary>
        /// 礼物连击
        /// </summary>
        COMBO_SEND,


        /// <summary>
        /// 舰长进入
        /// </summary>
        ENTRY_EFFECT,

        LIKE_INFO_V3_UPDATE,

        DANMU_AGGREGATION,
        LIKE_INFO_V3_CLICK,
        POPULARITY_RED_POCKET_WINNER_LIST,
        POPULARITY_RED_POCKET_START,

        /// <summary>
        /// 上舰之类
        /// </summary>
        GUARD_BUY,

        /// <summary>
        /// SC
        /// </summary>
        SUPER_CHAT_MESSAGE,


        USER_TOAST_MSG,

        SUPER_CHAT_MESSAGE_JPN,

        POPULAR_RANK_CHANGED,
    }
}
