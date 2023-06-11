using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BDanMuLib.Utils
{
    /// <summary>
    /// Emote工具类
    /// </summary>
    /// <remarks>
    /// 估计用不上
    /// </remarks>
    public class EmoteUtils
    {

        private EmoteUtils()
        {

        }

        private static readonly Lazy<Dictionary<string, string>> _emotes = new(() =>
        {
            var text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "emote.json"), Encoding.UTF8);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
        });

        public static string HandleCommentWithEmote(string comment, JObject extra)
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

                        comment = comment.Replace(emoji, "<img referrer=\"no-referrer\" height=\"20\" width=\"20\" src=\"" + url + "\"/>");
                    });
                });
            }

            //foreach (var item in _emotes)
            //{
            //    string emote = "[" + item.Key + "]";
            //    if (comment.Contains(emote))
            //    {
            //        comment = comment.Replace(emote, "<img referrer=\"no-referrer\" height=\"20\" width=\"20\" src=\"" + item.Value + "\"/>");

            //        return comment;
            //    }
            //}

            return comment;
        }
    }
}
