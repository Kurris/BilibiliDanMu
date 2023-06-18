using LiveCore.Enums;

namespace LiveCore.Models
{
    public class GuardBuyInfo : BaseInfo
    {
        public long Uid { get; set; }

        public string UserName { get; set; }
        public GuardType GuardType { get; set; }
        public int Num { get; set; }
        public int Price { get; set; }
        public string PriceString => (Price / 1000) * Num + " CNY";


        public int GiftId { get; set; }
        public string GiftName { get; set; }
    }
}
