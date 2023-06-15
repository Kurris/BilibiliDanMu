using System;
using System.Linq;
using System.Threading.Tasks;
using BDanmuLib.Enums;
using BDanMuLib.Emuns;
using BDanMuLib.Enums;
using BDanMuLib.Extensions;
using BDanMuLib.Models;
using BDanMuLib.Utils;
using Newtonsoft.Json.Linq;

namespace BDanMuLib.Services
{
    internal class RawtHandleService
    {
        private readonly EmoteService _emoteService;
        private readonly AvatarService _avatarService;
        private readonly GiftService _giftService;

        public RawtHandleService(EmoteService emoteService, AvatarService avatarService, GiftService giftService)
        {
            _emoteService = emoteService;
            _avatarService = avatarService;
            _giftService = giftService;
        }

        public async Task<Result> FromDanMuMsgAsync(JObject jObj)
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
            var uniqueComment = info[0][13];
            var extra = JObject.Parse(info[0][15]["extra"].Value<string>());

            //debug
            if (userName.Equals("kurris", StringComparison.OrdinalIgnoreCase))
            {

            }

            //表情处理
            comment = uniqueComment.HasValues
                ? _emoteService.HandleCommentEmoteUnique(uniqueComment, extra)
                : _emoteService.HandleCommentWithEmote(comment, extra);

            var medal = info[3];
            var hasMedal = medal.Any();

            return new Result(MessageType.DANMU_MSG, new BarrageInfo()
            {
                Mid = mid,
                FaceUrl = await _avatarService.GetByBilibiliUserId(mid),
                Comment = comment,
                IsAdmin = isAdmin,
                Time = time,
                UserName = userName,
                AudRank = audRank,
                HasMedal = hasMedal,
                MedalName = hasMedal ? medal[1].Value<string>() : string.Empty,
                MedalLevel = hasMedal ? medal[0].Value<int>() : 0,
                Top3 = top3,
                Guard = GuardType.CheckGuardByColor(color),
            });
        }

        public Result FromSuperChat(JObject jObj)
        {

            var info = jObj["data"];
            var medalInfo = info["medal_info"];

            var superChatInfo = new SuperChatInfo()
            {
                GiftId = info["gift"]["gift_id"].Value<int>(),
                GiftName = info["gift"]["gift_name"].Value<string>(),
                Num = info["gift"]["num"].Value<int>(),

                BackgroundBottomColor = info["background_bottom_color"].Value<string>(),
                BackgroundColor = info["background_color"].Value<string>(),
                BackgroundImage = info["background_image"].Value<string>(),
                BackgroundPriceColor = info["background_price_color"].Value<string>(),

                MedalColor = medalInfo.Any() ? info["medal_info"]["medal_color"].Value<string>() : string.Empty,
                MedalName = medalInfo.Any() ? info["medal_info"]["medal_name"].Value<string>() : string.Empty,
                MedalLevel = medalInfo.Any() ? info["medal_info"]["medal_level"].Value<int>() : 0,

                UserFace = info["user_info"]["face"].Value<string>(),
                UserFaceFrame = info["user_info"]["face_frame"].Value<string>(),
                UserName = info["user_info"]["uname"].Value<string>(),
                UserNameColor = info["user_info"]["name_color"].Value<string>(),


                Message = info["message"].Value<string>(),
                MessageFontColor = info["message_font_color"].Value<string>(),
                Price = info["price"].Value<int>(),
            };


            return new Result(MessageType.SUPER_CHAT_MESSAGE, superChatInfo);
        }

        public Result FromWatchedChanged(JObject jObj)
        {
            var watchedNum = jObj["data"]["num"].Value<int>();
            return new Result(MessageType.WATCHED_CHANGE, new WatchedInfo(watchedNum));
        }


        public Result FromInteractWord(JObject jObj)
        {
            var data = jObj["data"];
            var userName = data["uname"].Value<string>();

            return new Result(MessageType.INTERACT_WORD, new InteractWordInfo()
            {
                UserName = $"{userName} 进入直播间"
            });
        }

        public Result FromEntryEffect(JObject jObj)
        {
            var data = jObj["data"];
            var copyWriting = data["copy_writing"].Value<string>();

            copyWriting = copyWriting.Replace("<%", "<span style=\"color:yellow\">");
            copyWriting = copyWriting.Replace("%>", "</span>");


            return new Result(MessageType.ENTRY_EFFECT, new EntryEffectInfo()
            {
                Face = data["face"].Value<string>(),
                Message = copyWriting,
                BaseImageUrl = data["web_basemap_url"].Value<string>(),
            });
        }

        public Result FromSendGift(JObject jObj)
        {

            var data = jObj["data"];
            var uid = data["uid"].Value<long>();
            var userName = data["uname"].Value<string>();
            var action = data["action"].Value<string>();
            var giftId = data["giftId"].Value<int>();
            var giftName = data["giftName"].Value<string>();
            var price = data["price"].Value<int>();
            var num = data["num"].Value<int>();


            var coinType = CoinType.CheckCoinType(data["coin_type"].Value<string>());
            var gifUrl = _giftService.GetGifUrl(giftId);

            return new Result(MessageType.SEND_GIFT, new SendGiftInfo()
            {
                Uid = uid,
                From = userName,
                GiftName = giftName,
                Price = price,
                Num = num,
                GifUrl = gifUrl,
                CoinType = coinType
            });
        }

        public Result FromGuardBuy(JObject jObj)
        {

            var data = jObj["data"];

            return new Result(MessageType.GUARD_BUY, new GuardBuyInfo()
            {
                Uid = data["uid"].Value<long>(),
                UserName = data["username"].Value<string>(),
                GiftName = data["gift_name"].Value<string>(),
                Price = data["price"].Value<int>(),
                Num = data["num"].Value<int>(),
                GiftId = data["gift_id"].Value<int>(),
                GuardType = GuardType.CheckGuardByLevel(data[key: "guard_level"].Value<int>())
            });
        }

        public Result FromUserToastMsg(JObject jObj)
        {
            //price为实际金瓜子标价，即rmb*1000

            var data = jObj["data"];

            var msg = data["toast_msg"].Value<string>();
            var userName = data["username"].Value<string>();
            var unit = data["unit"].Value<string>();
            var uid = data["uid"].Value<long>();
            var roleName = data["role_name"].Value<string>();
            var num = data["num"].Value<int>();
            var guardType = data["guard_level"].Value<int>();
            var effectId = data["effect_id"].Value<int>();
            var targetGuardCount = data["target_guard_count"].Value<int>();
            var price = data["price"].Value<int>();
            var priceString = (price / 1000) * num + " CNY";

            return new Result(MessageType.USER_TOAST_MSG, new UserToastMsgInfo
            {
                Uid = uid,
                UserName = userName,
                Unit = unit,
                RoleName = roleName,
                Num = num,
                GuardType = GuardType.CheckGuardByLevel(guardType),
                EffectId = effectId,
                TargetGuardCount = targetGuardCount,
                PriceString = priceString,
                Price = price,
                Message = msg,
            });
        }
    }
}
