using System;
using System.Linq;
using System.Threading.Tasks;
using BDanMuLib.Emuns;
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
                Price = info["price"].Value<int>() * 10,
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
            var userName = data["uname"].Value<string>();
            var action = data["action"].Value<string>();
            var giftId = data["giftId"].Value<int>();
            var giftName = data["giftName"].Value<string>();
            var price = data["price"].Value<int>();
            var num = data["num"].Value<int>();

            var gifUrl = GiftUtils.GetGifUrl(giftId);

            return new SendGiftInfo()
            {
                From = userName,
                GiftName = giftName,
                Price = price,
                Num = num,
                GifUrl = gifUrl
            };
        }
    }
}
