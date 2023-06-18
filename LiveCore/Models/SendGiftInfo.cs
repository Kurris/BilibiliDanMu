using LiveCore.Enums;

namespace LiveCore.Models
{
    public class SendGiftInfo : BaseInfo
    {
        public long Uid { get; set; }
        public string From { get; set; }
        public string GiftName { get; set; }
        public int Price { get; set; }
        public int Num { get; set; }
        public string GifUrl { get; set; }

        public CoinType CoinType { get; set; }

        public int TotalCoin { get; set; }
    }
}

