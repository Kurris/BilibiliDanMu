﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LiveCore.Consts;
using LiveCore.Enums;
using LiveCore.Models;
using Newtonsoft.Json.Linq;
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable PossibleNullReferenceException

namespace LiveCore.Services;

public class BilibiliApiService
{

    private readonly HttpClient _httpClient;

    public BilibiliApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<BroadCastInfo> GetBroadCastInfoAsync(int roomId)
    {
        var requestContent = await _httpClient.GetStringAsync(BilibiliApiUrlConsts.BroadCastUrl + roomId);
        var data = JObject.Parse(requestContent)["data"]!;

        return new BroadCastInfo()
        {
            Token = data["token"]!.Value<string>(),
            Host = data["host"]!.Value<string>(),
            Port = data["port"]!.Value<int>(),
        };
    }

    public async Task<string> GetUserAvatarFromSpaceHtmlAsync(string mid)
    {
        try
        {
            string response = await _httpClient.GetStringAsync($"https://space.bilibili.com/{mid}");
            int i = response.IndexOf("href=\"//i0.hdslb.com", StringComparison.Ordinal);
            if (i == -1)
            {
                i = response.IndexOf("href=\"//i1.hdslb.com", StringComparison.Ordinal);
                if (i == -1)
                {
                    i = response.IndexOf("href=\"//i2.hdslb.com", StringComparison.Ordinal);
                }
            }

            if (i == -1)
            {
                //请求个人信息api
            }
            else
            {
                response = response[i..];
                i = response.IndexOf(".jpg\">", StringComparison.Ordinal);
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


    public async Task<RoomInfo> GetRoomInfoAsync(int roomId)
    {
        var response = await _httpClient.GetStringAsync(BilibiliApiUrlConsts.RoomInfoUrl + roomId);
        var jObj = JObject.Parse(response);
        var code = jObj["code"].Value<int>();
        if (code != 0)
        {
            return null;
        }
        var data = jObj["data"];

        return new RoomInfo
        {
            Uid = data["uid"].Value<long>(),
            RoomId = data["room_id"].Value<int>(),
            ShortRoomId = data["short_id"].Value<int>(),
            Title = data["title"].Value<string>(),
            Tags = data["tags"].Value<string>().Split(','),
            Description = data["description"].Value<string>(),
            ParentAreaId = data["parent_area_id"].Value<int>(),
            ParentAreaName = data["parent_area_name"].Value<string>(),
            AreaId = data["area_id"].Value<int>(),
            AreaName = data["area_name"].Value<string>(),
            LiveStatus = (LiveStatusType)data["live_status"].Value<int>(),
            LiveTime = data["live_status"].Value<int>() != 1 ? null : data["live_time"].Value<DateTime>(),
            BackgroundUrl = data["background"].Value<string>(),
            KeyFrameUrl = data["keyframe"].Value<string>(),
            UserCoverUrl = data["user_cover"].Value<string>(),
        };
    }


    /// <summary>
    /// api获取礼物信息(不完整),需要使用GiftUtils
    /// </summary>
    /// <param name="roomId"></param>
    /// <returns></returns>
    public async Task<List<GiftInfo>> GetGiftListAsync(int roomId)
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
        string url = string.Concat(BilibiliApiUrlConsts.GiftListUrl, "?", p);

        var response = await _httpClient.GetStringAsync(url);
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



        response = await _httpClient.GetStringAsync("https://api.live.bilibili.com/xlive/web-room/v1/giftPanel/roomGiftConfig?platform=pc&global_version=" + timestamp);

        jObj = JObject.Parse(response);

        data = jObj["data"];
        list = data["list"];

        list.ToList().ForEach(j =>
        {
            if (giftList.All(x => x.Id != j["id"].Value<int>()))
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




    public async Task<string> GetBroadCastStreamUrlAsync(int roomId)
    {
        var roomInfo = await GetRoomInfoAsync(roomId);

        using var content = new FormUrlEncodedContent(new Dictionary<string, string>()
        {
            ["cid"] = roomInfo.RoomId.ToString(),
            ["platform"] = "web", //h5:hls方式  , web:http-flv方式
            ["quality"] = "4", //2.流畅 3.高清 4.原画
            //["qn"] = 80：流畅 150：高清 400：蓝光 10000：原画 20000：4K 30000：杜比
        });

        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Referer", "https://www.bilibili.com");
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36");
        client.DefaultRequestHeaders.Add("Origin", "https://www.bilibili.com");
        var response = await client.GetStringAsync(BilibiliApiUrlConsts.BroadCastStreamUrl + "?" + await content.ReadAsStringAsync());
        var jObj = JObject.Parse(response);
        var urlInfos = jObj["data"]["durl"];
        var url = urlInfos[0]["url"].Value<string>();

        //response = await _client.GetStringAsync("https://api.live.bilibili.com/xlive/web-room/v2/index/getRoomPlayInfo?room_id=6750632&protocol=0,1&format=0,1,2&codec=0,1&qn=10000");
        //jObj = JObject.Parse(response);

        return url;
    }

    public async Task<StreamerInfo> GetStreamerInfoAsync(long mid)
    {
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>()
        {
            ["uid"] = mid.ToString(),
        });

        var response = await _httpClient.GetStringAsync(BilibiliApiUrlConsts.StreamerInfoUrl + "?" + await content.ReadAsStringAsync());
        var jObj = JObject.Parse(response);
        var data = jObj["data"];

        var info = data["info"];

        var userName = info["uname"].Value<string>();
        var face = info["face"].Value<string>();
        //-1：保密,0：女 ,1：男
        var gender = info["gender"].Value<int>();

        var level = data["exp"]["master_level"]["level"].Value<int>();
        var followerNum = data["follower_num"].Value<int>();

        var pendant = data["pendant"].Value<string>();

        var board = data["room_news"]["content"].Value<string>();

        return new StreamerInfo()
        {
            Uid = mid,
            UserName = userName,
            Face = face,
            Gender = gender,
            Level = level,
            FollowerNum = followerNum,
            Pendant = pendant,
            BoardMessage = board
        };
    }
}