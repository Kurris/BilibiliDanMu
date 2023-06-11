using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BDanMuLib.Models;
using Newtonsoft.Json.Linq;

namespace BDanMuLib.Utils
{
    public class GiftUtils
    {
        private static readonly Lazy<List<GiftInfo>> _giftInfos = new(() =>
        {
            var text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "gift.json"), Encoding.UTF8);
            var jObj = JObject.Parse(text);

            var globalGift = jObj["global_gift"];
            var list = globalGift["list"];


            var giftList = new List<GiftInfo>(list.Count());

            list.ToList().ForEach(j =>
            {
                giftList.Add(new GiftInfo()
                {
                    Id = j["id"].Value<int>(),
                    Name = j["name"].Value<string>(),
                    Price = j["price"].Value<int>(),
                    Description = j["desc"].Value<string>(),
                    Gif = j["gif"].Value<string>(),
                });
            });



            list = jObj["list"];

            list.ToList().ForEach(j =>
            {
                if (!giftList.Any(x => x.Id == j["id"].Value<int>()))
                {
                    giftList.Add(new GiftInfo()
                    {
                        Id = j["id"].Value<int>(),
                        Name = j["name"].Value<string>(),
                        Price = j["price"].Value<int>(),
                        Description = j["desc"].Value<string>(),
                        Gif = j["gif"].Value<string>(),
                    });
                }
            });



            return giftList.OrderBy(x => x.Id).ToList();
        });

        public static List<GiftInfo> GiftInfos => _giftInfos.Value;
    }
}
