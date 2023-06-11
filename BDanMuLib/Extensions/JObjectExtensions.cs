using System;
using System.Linq;
using System.Threading.Tasks;
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
            //#e5f1f9

            //debug
            if (userName.Equals("kurris", StringComparison.OrdinalIgnoreCase))
            {

            }

            var extra = JObject.Parse(info[0][15]["extra"].Value<string>());

            //处理点击即可发送的弹幕表情(不显示在文本框中的表情)
            if (info[0][13].HasValues)
            {
                var emoteUnique = info[0][13]["emoticon_unique"].Value<string>();
                var width = info[0][13]["width"].Value<int>();
                var height = info[0][13]["height"].Value<int>();
                if (width == height)
                {
                    width = 40;
                    height = 40;
                }
                else
                {
                    height /= 2;
                    width /= 2;
                }

                var emoticon_unique = extra["emoticon_unique"].Value<string>();
                if (emoticon_unique == emoteUnique)
                {
                    var url = info[0][13]["url"].Value<string>();
                    comment = "<img referrer=\"no-referrer\" height=\"" + height + "\" width=\"" + width + "\" src=\"" + url + "\"/>";
                }
            }
            else
            {
                //处理弹幕中的表情一般为[dog]格式
                comment = EmoteUtils.HandleCommentWithEmote(comment, extra);
            }

            var medal = info[3];
            var hasMedal = medal.Any();
            string medalName = null;
            int level = 0;
            if (hasMedal)
            {
                medalName = medal[1].Value<string>();
                level = medal[0].Value<int>();
            }

            string faceUrl = await RequestUtils.GetUserAvatarFromSpaceHtmlAsync(mid);

            return new BarrageInfo()
            {
                Mid = mid,
                FaceUrl = faceUrl,
                Comment = comment,
                IsAdmin = isAdmin,
                Time = time,
                UserName = userName,
                AudRank = audRank,
                HasMedal = hasMedal,
                MedalName = medalName,
                MedalLevel = level,
                Top3 = top3,
                Color = color,
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


            //sc channel speak
            _ = ChannelUtils.PublishAsync(string.Concat(superChatInfo.UserName, "发送了", superChatInfo.Num, "条", superChatInfo.GiftName));
            _ = ChannelUtils.PublishAsync(superChatInfo.Message);

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
    }
}
