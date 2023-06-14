using System;
using System.Linq;
using System.Threading.Tasks;
using BDanMuLib.Emuns;
using BDanMuLib.Enums;
using BDanMuLib.Models;
using BDanMuLib.Utils;
using Newtonsoft.Json.Linq;

namespace BDanMuLib.Extensions
{
    public static class JObjectExtensions
    {

        public static async Task<BarrageInfo> FromDanMuMsgAsync(this JObject jObj)
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

            if (!string.IsNullOrEmpty(color) && !new[] { "#00D1F1", "#E17AFF" }.Contains(color))
            {
                Console.WriteLine(userName + " : " + color);
            }

            //debug
            if (userName.Equals("kurris", StringComparison.OrdinalIgnoreCase))
            {
            }

            //表情处理
            comment = uniqueComment.HasValues
                ? comment = EmoteUtils.HandleCommentEmoteUnique(uniqueComment, extra)
                : comment = EmoteUtils.HandleCommentWithEmote(comment, extra);

            var medal = info[3];
            var hasMedal = medal.Any();

            return new BarrageInfo()
            {
                Mid = mid,
                FaceUrl = await AvatarUtils.Get(mid),
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
            };
        }

        public static SuperChatInfo FromSuperChat(this JObject jObj)
        {
            #region debug
            //            jObj = JObject.Parse(@"{
            //    ""cmd"": ""SUPER_CHAT_MESSAGE"",
            //    ""data"": {
            //        ""background_bottom_color"": ""#2A60B2"",
            //        ""background_color"": ""#EDF5FF"",
            //        ""background_color_end"": ""#405D85"",
            //        ""background_color_start"": ""#3171D2"",
            //        ""background_icon"": """",
            //        ""background_image"": ""https://i0.hdslb.com/bfs/live/a712efa5c6ebc67bafbe8352d3e74b820a00c13e.png"",
            //        ""background_price_color"": ""#7497CD"",
            //        ""color_point"": 0.7,
            //        ""dmscore"": 112,
            //        ""end_time"": 1654978081,
            //        ""gift"": {
            //            ""gift_id"": 12000,
            //            ""gift_name"": ""醒目留言"",
            //            ""num"": 1
            //        },
            //        ""id"": 4258549,
            //        ""is_ranked"": 1,
            //        ""is_send_audit"": 0,
            //        ""medal_info"": null,
            //        ""message"": ""hana等会真的唱 向天再借五百年吗"",
            //        ""message_font_color"": ""#A3F6FF"",
            //        ""message_trans"": """",
            //        ""price"": 30,
            //        ""rate"": 1000,
            //        ""start_time"": 1654978021,
            //        ""time"": 60,
            //        ""token"": ""3A85D663"",
            //        ""trans_mark"": 0,
            //        ""ts"": 1654978021,
            //        ""uid"": 109319072,
            //        ""user_info"": {
            //            ""face"": ""http://i2.hdslb.com/bfs/face/de172ea5b6f3d09769fe3b0e5e6e57c02350d49d.jpg"",
            //            ""face_frame"": """",
            //            ""guard_level"": 0,
            //            ""is_main_vip"": 1,
            //            ""is_svip"": 0,
            //            ""is_vip"": 0,
            //            ""level_color"": ""#969696"",
            //            ""manager"": 0,
            //            ""name_color"": ""#666666"",
            //            ""title"": ""0"",
            //            ""uname"": ""AyeUniCute"",
            //            ""user_level"": 0
            //        }
            //    },
            //    ""roomid"": 9015372
            //}");

            #endregion

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


            //var speakText = string.Concat(superChatInfo.UserName, "发送了", superChatInfo.Num, "条", superChatInfo.GiftName, ",", superChatInfo.Message);
            //sc channel speak
            //windows only
            //_ = ChannelUtils.PublishAsync();

            return superChatInfo;
        }

        public static WatchedInfo FromWatchedChanged(this JObject jObj)
        {
            var watchedNum = jObj["data"]["num"].Value<int>();
            return new WatchedInfo(watchedNum);
        }


        public static InteractWordInfo FromInteractWord(this JObject jObj)
        {
            var data = jObj["data"];
            var userName = data["uname"].Value<string>();

            return new InteractWordInfo()
            {
                UserName = $"{userName} 进入直播间"
            };
        }

        public static EntryEffectInfo FromEntryEffect(this JObject jObj)
        {
            var data = jObj["data"];
            var copyWriting = data["copy_writing"].Value<string>();

            copyWriting = copyWriting.Replace("<%", "<span style=\"color:yellow\">");
            copyWriting = copyWriting.Replace("%>", "</span>");


            return new EntryEffectInfo()
            {
                Face = data["face"].Value<string>(),
                Message = copyWriting,
                BaseImageUrl = data["web_basemap_url"].Value<string>(),
            };
        }

        public static SendGiftInfo FromSendGift(this JObject jObj)
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
            var gifUrl = GiftUtils.GetGifUrl(giftId);

            return new SendGiftInfo()
            {
                Uid = uid,
                From = userName,
                GiftName = giftName,
                Price = price,
                Num = num,
                GifUrl = gifUrl,
                CoinType = coinType
            };
        }

        public static GuardBuyInfo FromGuardBuy(this JObject jObj)
        {

            var data = jObj["data"];


            return new GuardBuyInfo()
            {
                Uid = data["uid"].Value<long>(),
                UserName = data["username"].Value<string>(),
                GiftName = data["gift_name"].Value<string>(),
                Price = data["price"].Value<int>(),
                Num = data["num"].Value<int>(),
                GiftId = data["gift_id"].Value<int>(),
                GuardType = GuardType.CheckGuardByLevel(data[key: "guard_level"].Value<int>())
            };
        }

        public static UserToastMsgInfo FromUserToastMsg(this JObject jObj)
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

            return new UserToastMsgInfo
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
            };
        }
    }
}
