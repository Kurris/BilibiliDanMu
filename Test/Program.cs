

using BDanmuLib.Models;
using BDanMuLib;
using BDanMuLib.Extensions;
using BDanMuLib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var danmuCore = new DanMuCore();
danmuCore.ReceiveMessage += DanmuCore_ReceiveMessage;

int roomId = 6750632;

//"突出一个三十分钟前没这个人".Speak();

//var connect = danmuCore.ConnectAsync(roomId).Result;


//sc test
//string json = "{\"cmd\":\"SUPER_CHAT_MESSAGE\",\"data\":{\"background_bottom_color\":\"#2A60B2\",\"background_color\":\"#EDF5FF\",\"background_color_end\":\"#405D85\",\"background_color_start\":\"#3171D2\",\"background_icon\":\"\",\"background_image\":\"https://i0.hdslb.com/bfs/live/a712efa5c6ebc67bafbe8352d3e74b820a00c13e.png\",\"background_price_color\":\"#7497CD\",\"color_point\":0.7,\"dmscore\":112,\"end_time\":1654978081,\"gift\":{\"gift_id\":12000,\"gift_name\":\"醒目留言\",\"num\":1},\"id\":4258549,\"is_ranked\":1,\"is_send_audit\":0,\"medal_info\":null,\"message\":\"hana,等会真的唱,向天再借五百年吗\",\"message_font_color\":\"#A3F6FF\",\"message_trans\":\"\",\"price\":30,\"rate\":1000,\"start_time\":1654978021,\"time\":60,\"token\":\"3A85D663\",\"trans_mark\":0,\"ts\":1654978021,\"uid\":109319072,\"user_info\":{\"face\":\"http://i2.hdslb.com/bfs/face/de172ea5b6f3d09769fe3b0e5e6e57c02350d49d.jpg\",\"face_frame\":\"\",\"guard_level\":0,\"is_main_vip\":1,\"is_svip\":0,\"is_vip\":0,\"level_color\":\"#969696\",\"manager\":0,\"name_color\":\"#666666\",\"title\":\"0\",\"uname\":\"AyeUniCute\",\"user_level\":0}},\"roomid\":9015372}";

//var jObject = JObject.Parse(json);
//var info = jObject.FromSuperChat();
//new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.AsEnumerable().AsParallel().ForAll(x =>
//{
//    Console.WriteLine(x);
//    jObject.FromSuperChat();
//    Console.WriteLine("done:" + x);
//});


var roomInfo = RequestUtils.GetRoomInfoAsync(roomId).Result;

Console.ReadKey();

static void DanmuCore_ReceiveMessage(MessageType messageType, object obj)
{
    if (messageType == MessageType.DANMU_MSG)
    {
        Console.WriteLine($"{messageType}:{JsonConvert.SerializeObject(obj)}");
    }

    //if (messageType == MessageType.SEND_GIFT || messageType == MessageType.ENTRY_EFFECT || messageType == MessageType.GUARD_BUY)
    //{
    //    Console.WriteLine($"{messageType}:{obj}");
    //}
}
