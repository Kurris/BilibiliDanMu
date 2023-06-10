using System;
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
            var response = await _client.GetStringAsync(ApiUrls.GetRoomInfoUrl + roomId);
            var jObj = JObject.Parse(response);
            var data = jObj["data"];

            return new RoomInfo()
            {
                RoomId = data["room_id"].Value<int>(),
                ShortRoomId = data["short_id"].Value<int>(),
                ParentAreaId = data["parent_area_id"].Value<int>(),
                ParentAreaName = data["parent_area_name"].Value<string>(),
                AreaId = data["area_id"].Value<int>(),
                AreaName = data["area_name"].Value<string>(),
            };
        }

    }
}
