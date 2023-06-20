using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LiveCore.Models;
using Newtonsoft.Json.Linq;

namespace LiveCore.Services
{
    /// <summary>
    /// singletion
    /// </summary>
    public class GiftService
    {
        private static readonly Dictionary<int, GiftInfo> _dicGiftInfos;

        static GiftService()
        {
            var text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "gift.json"), Encoding.UTF8);
            var jObj = JObject.Parse(text);

            var globalGift = jObj["global_gift"]!;
            var list = globalGift["list"]!;


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



            list = jObj["list"]!;

            list.ToList().ForEach(j =>
            {
                if (giftList.All(x => x.Id != j["id"]!.Value<int>()))
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

            _dicGiftInfos = giftList.ToDictionary(x => x.Id, x => x); ;
        }

        public string GetGifUrl(int giftId)
        {
            var exists = _dicGiftInfos.TryGetValue(giftId, out var info);
            return exists ? info.Gif : string.Empty;
        }
    }
}
