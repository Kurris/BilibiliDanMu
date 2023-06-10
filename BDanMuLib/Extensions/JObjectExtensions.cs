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

            if (info[0][13].Any())
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
                var extra = JObject.Parse(info[0][15]["extra"].Value<string>());
                var emoticon_unique = extra["emoticon_unique"].Value<string>();

                if (emoticon_unique == emoteUnique)
                {
                    var url = info[0][13]["url"].Value<string>();
                    comment = "<img referrer=\"no-referrer\" height=\"" + height + "\" width=\"" + width + "\" src=\"" + url + "\"/>";
                }
            }
            else
            {
                comment = EmoteUtils.HandleCommentWithEmote(comment);
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
    }
}
