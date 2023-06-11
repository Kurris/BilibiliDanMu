using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BDanMuLib.Models;
using Newtonsoft.Json.Linq;

namespace BDanMuLib.Utils
{
    public class RequestUtils
    {

        private readonly static HttpClient _client;
        static RequestUtils()
        {
            _client = new HttpClient();
        }

        private RequestUtils()
        {

        }

        public static async Task<BroadCastInfo> GetBroadCastInfoAsync(int roomId)
        {
            var requestContent = await _client.GetStringAsync(ApiUrls.BroadCastUrl + roomId);
            var dataJToken = JObject.Parse(requestContent)["data"];

            return new BroadCastInfo()
            {
                Token = dataJToken["token"].Value<string>(),
                Host = dataJToken["host"].Value<string>(),
                Port = dataJToken["port"].Value<int>(),
            };
        }

        public static async Task<string> GetUserAvatarFromSpaceHtmlAsync(string mid)
        {
            try
            {
                string response = await _client.GetStringAsync($"https://space.bilibili.com/{mid}");
                int i = response.IndexOf("href=\"//i0.hdslb.com");
                if (i == -1)
                {
                    i = response.IndexOf("href=\"//i1.hdslb.com");
                    if (i == -1)
                    {
                        i = response.IndexOf("href=\"//i2.hdslb.com");
                    }
                }

                if (i == -1)
                {
                    //请求个人信息api
                }
                else
                {
                    response = response[i..];
                    i = response.IndexOf(".jpg\">");
                    if (i != -1)
                    {
                        return string.Concat("http:", response.AsSpan(6, i - 2));
                    }
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }


        public static async Task<RoomInfo> GetRoomInfoAsync(int roomId)
        {
            var response = await _client.GetStringAsync(ApiUrls.RoomInfoUrl + roomId);
            var jObj = JObject.Parse(response);
            var data = jObj["data"];

            return new RoomInfo()
            {
                RoomId = data["room_id"].Value<int>(),
                ShortRoomId = data["short_id"].Value<int>(),
                Title = data["title"].Value<string>(),
                Tags = data["tags"].Value<string>().Split(','),
                Description = data["description"].Value<string>(),
                ParentAreaId = data["parent_area_id"].Value<int>(),
                ParentAreaName = data["parent_area_name"].Value<string>(),
                AreaId = data["area_id"].Value<int>(),
                AreaName = data["area_name"].Value<string>(),
                LiveStatus = data["live_status"].Value<int>(),
            };
        }


        /// <summary>
        /// api获取礼物信息(不完整),需要使用GiftUtils
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public static async Task<List<GiftInfo>> GetGiftListAsync(int roomId)
        {
            var roomInfo = await GetRoomInfoAsync(roomId);

            //platform=pc&room_id=6750632&area_parent_id=2&area_id=92&source=live&build=0&global_version=
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            using var content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                ["platform"] = "pc",
                ["room_id"] = roomInfo.RoomId.ToString(),
                ["area_parent_id"] = roomInfo.ParentAreaId.ToString(),
                ["area_id"] = roomInfo.AreaId.ToString(),
                ["source"] = "live",
                ["build"] = "0",
                ["global_version"] = timestamp.ToString()
            });

            var p = await content.ReadAsStringAsync();
            string url = string.Concat(ApiUrls.GiftListUrl, "?", p);

            var response = await _client.GetStringAsync(url);
            var jObj = JObject.Parse(response);

            var data = jObj["data"];
            var list = data["list"];


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



            response = await _client.GetStringAsync("https://api.live.bilibili.com/xlive/web-room/v1/giftPanel/roomGiftConfig?platform=pc&global_version=" + timestamp);

            jObj = JObject.Parse(response);

            data = jObj["data"];
            list = data["list"];

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

            return giftList.OrderBy(j => j.Id).ToList();

        }

    }
}
