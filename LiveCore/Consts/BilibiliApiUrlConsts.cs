﻿namespace LiveCore.Consts
{
    internal static class BilibiliApiUrlConsts
    {
        /// <summary>
        /// 用户信息(容易被ban ip)
        /// </summary>
        public const string UserInfoUrl = "https://api.bilibili.com/x/space/acc/info?mid=";


        /// <summary>
        /// 用户直播地址
        /// </summary>
        public const string BroadCastUrl = "https://api.live.bilibili.com/room/v1/Danmu/getConf?room_id=";


        /// <summary>
        /// 获取房间信息
        /// </summary>
        public const string RoomInfoUrl = "https://api.live.bilibili.com/room/v1/Room/get_info?room_id=";

        /// <summary>
        /// 获取礼物列表
        /// </summary>
        public const string GiftListUrl = "https://api.live.bilibili.com/xlive/web-room/v1/giftPanel/roomGiftConfig";


        /// <summary>
        /// 表情包地址
        /// </summary>
        public const string EmoteUrl = "https://api.bilibili.com/x/emote/setting/panel?business=reply";

        /// <summary>
        /// 直播流地址
        /// </summary>
        public const string BroadCastStreamUrl = "https://api.live.bilibili.com/room/v1/Room/playUrl";


        /// <summary>
        /// 主播信息地址
        /// </summary>
        public const string StreamerInfoUrl = "https://api.live.bilibili.com/live_user/v1/Master/info";
    }
}
