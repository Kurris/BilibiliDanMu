using System.Linq;
using Newtonsoft.Json.Linq;

namespace BDanMuLib.Services
{
    internal class EmoteService
    {
        /// <summary>
        /// 处理弹幕中的表情一般为[dog]格式
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="extra"></param>
        /// <returns></returns>
        public string HandleCommentWithEmote(string comment, JObject extra)
        {
            var emots = extra["emots"];
            if (emots.HasValues)
            {
                emots.ToList().ForEach(x =>
                {
                    x.ToList().ForEach(p =>
                    {
                        var emoji = p["emoji"].Value<string>();
                        var url = p["url"].Value<string>();

                        comment = comment.Replace(emoji, "<img style=\"vertical-align:text-top;\"  referrer=\"no-referrer\" height=\"20\" width=\"20\" src=\"" + url + "\"/>");
                    });
                });
            }

            return comment;
        }

        /// <summary>
        /// 处理点击即可发送的弹幕表情(不显示在文本框中的表情)
        /// </summary>
        /// <param name="uniqueComment"></param>
        /// <param name="extra"></param>
        /// <returns></returns>
        public string HandleCommentEmoteUnique(JToken uniqueComment, JObject extra)
        {
            var emoteUnique = uniqueComment["emoticon_unique"].Value<string>();
            var width = uniqueComment["width"].Value<int>();
            var height = uniqueComment["height"].Value<int>();
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
                var url = uniqueComment["url"].Value<string>();
                return "<img referrer=\"no-referrer\" height=\"" + height + "\" width=\"" + width + "\" src=\"" + url + "\"/>";
            }
            return string.Empty;
        }
    }
}
