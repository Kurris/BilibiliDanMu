using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace BDanMuLib.Utils
{
    public class EmoteUtils
    {
        /// <summary>
        /// 头像
        /// </summary>
        private readonly static Dictionary<string, string> _emotes;


        static EmoteUtils()
        {
            var text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "emote.json"), Encoding.UTF8);
            _emotes = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
        }

        private EmoteUtils()
        {

        }

        public static string HandleCommentWithEmote(string comment)
        {
            foreach (var item in _emotes)
            {
                string emote = "[" + item.Key + "]";
                if (comment.Contains(emote))
                {
                    comment = comment.Replace(emote, "<img referrer=\"no-referrer\" height=\"20\" width=\"20\" src=\"" + item.Value + "\"/>");

                    return comment;
                }
            }

            return comment;
        }
    }
}
