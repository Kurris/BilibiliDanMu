using System;
using BilibiliDanMuLib;
using Newtonsoft.Json.Linq;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            BilibiliDanMu danMu = new BilibiliDanMu();
            danMu.ConnectAsync(21301839).Wait();
            danMu.Log += DanMu_Log;
            //var jobj = JObject.Parse(dmjson);
            //var Cmd = jobj.Value<string>("cmd");

            //var infos= jobj["info"];

            Console.ReadKey();
        }

        private static void WriteInter(JToken token)
        {
            string user = token["uname"].Value<string>();

            Console.WriteLine($"{user}进入直播间");
        }

        private static void WriteDM(JToken token)
        {
            Console.WriteLine($"{token[2][1]}:{token[1]}");
        }


        private static string dmjson = "{\"cmd\":\"DANMU_MSG\",\"info\":[[0,4,25,16370799,1599548229402,1599548065,0,\"20bd3985\",0,0,5,\"#1953BAFF,#3353BAFF,#3353BAFF\"],\"这伽马P3？\",[282051187,\"Miko天下第一\",0,0,0,10000,1,\"#00D1F1\"],[21,\"橘安人\",\"喜欢草莓的橘子可\",21301839,1725515,\"\",0,6809855,1725515,5414290,3,1,22871593],[21,0,5805790,\"\\u003e50000\"],[\"\",\"\"],0,3,null,{\"ts\":1599548229,\"ct\":\"57E74A10\"},0,0,null,null,0]}";


        private static string inter = "    { \"cmd\":\"INTERACT_WORD\",\"data\":{ \"uid\":436453887,\"uname\":\"Hoy12356\",\"uname_color\":\"\",\"identities\":[1],\"msg_type\":1,\"roomid\":21301839,\"timestamp\":1599548357,\"score\":1599537357997193816,\"fans_medal\":{ \"target_id\":0,\"medal_level\":0,\"medal_name\":\"\",\"medal_color\":0,\"medal_color_start\":0,\"medal_color_end\":0,\"medal_color_border\":0,\"is_lighted\":0,\"guard_level\":0,\"special\":\"\",\"icon_id\":0} } }";

        private static void DanMu_Log(LogArgs e)
        {
            var jobj = JObject.Parse(e.Msg);
            var Cmd = jobj.Value<string>("cmd");

            var CmdCommand = (Cmd)Enum.Parse(typeof(Cmd), Cmd);

            switch (CmdCommand)
            {
                case BilibiliDanMuLib.Cmd.DANMU_MSG:

                    var infos = jobj["info"];
                    WriteDM(infos);
                    break;
                //case BilibiliDanMuLib.Cmd.SEND_GIFT:
                //    break;
                //case BilibiliDanMuLib.Cmd.WELCOME:
                //    break;
                //case BilibiliDanMuLib.Cmd.WELCOME_GUARD:
                //    break;
                //case BilibiliDanMuLib.Cmd.SYS_MSG:
                //    break;
                //case BilibiliDanMuLib.Cmd.PREPARING:
                //    break;
                //case BilibiliDanMuLib.Cmd.LIVE:
                //    break;
                //case BilibiliDanMuLib.Cmd.WISH_BOTTLE:
                //    break;
                case BilibiliDanMuLib.Cmd.INTERACT_WORD:
                    var DataJToken = jobj["data"];
                    WriteInter(DataJToken);
                    break;
                default:
                    Console.WriteLine(e.Msg);
                    break;
            }

        }
    }
}
