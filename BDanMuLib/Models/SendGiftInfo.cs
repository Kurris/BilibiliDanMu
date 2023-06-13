using System;
namespace BDanMuLib.Models
{
    public class SendGiftInfo : BaseInfo
    {
        public string From { get; set; }
        public string GiftName { get; set; }
        public int Price { get; set; }
        public int Num { get; set; }
        public string GifUrl { get; set; }
    }
}

