using System;
using System.Speech.Synthesis;
using BilibiliDanMuLib;
using Newtonsoft.Json.Linq;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            BilibiliDanMu danMu = new BilibiliDanMu();
            danMu.ConnectAsync(545072).Wait();
            danMu.OutPut += DanMu_OutPut;
            Console.ReadKey();
        }

        private static void DanMu_OutPut(Cmd cmd, string Msg)
        {
            Console.WriteLine(Msg);
        }


        private string aa = "{\"cmd\":\"SEND_GIFT\",\"data\":{\"draw\":0,\"gold\":0,\"silver\":0,\"num\":1,\"total_coin\":0,\"effect\":0,\"broadcast_id\":0,\"crit_prob\":0,\"guard_level\":0,\"rcost\":5877847,\"uid\":3338446,\"timestamp\":1599565592,\"giftId\":30607,\"giftType\":5,\"eventScore\":0,\"eventNum\":0,\"addFollow\":0,\"super\":0,\"super_gift_num\":0,\"super_batch_gift_num\":0,\"remain\":0,\"price\":0,\"newMedal\":0,\"newTitle\":0,\"title\":\"\",\"medal\":[],\"beatId\":\"0\",\"biz_source\":\"live\",\"metadata\":\"\",\"action\":\"投喂\",\"coin_type\":\"silver\",\"uname\":\"低調の↘低调\",\"face\":\"http://i2.hdslb.com/bfs/face/c24e0860cf7a69ecf2025e56362401a50a933d46.jpg\",\"batch_combo_id\":\"\",\"rnd\":\"1599561681\",\"giftName\":\"小心心\",\"notice_msg\":[],\"smalltv_msg\":[],\"combo_send\":null,\"batch_combo_send\":null,\"tag_image\":\"\",\"top_list\":[],\"send_master\":null,\"is_first\":true,\"demarcation\":1,\"combo_stay_time\":3,\"combo_total_coin\":1,\"tid\":\"1599565592112600001\",\"effect_block\":1,\"smallTVCountFlag\":true,\"capsule\":null,\"specialGift\":null,\"is_special_batch\":0,\"combo_resources_id\":1,\"magnification\":1,\"name_color\":\"\",\"medal_info\":{\"target_id\":0,\"special\":\"\",\"icon_id\":0,\"anchor_uname\":\"\",\"anchor_roomid\":0,\"medal_level\":0,\"medal_name\":\"\",\"medal_color\":0,\"medal_color_start\":0,\"medal_color_end\":0,\"medal_color_border\":0,\"is_lighted\":0,\"guard_level\":0},\"svga_block\":0}}";
    }
}
