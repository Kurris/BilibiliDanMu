using LiveCore.Enums;

namespace LiveCore.Models
{
    public class UserToastMsgInfo : BaseInfo
    {
        public long Uid { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string Unit { get; set; }
        public string RoleName { get; set; }
        public int Num { get; set; }
        public int Price { get; set; }
        public string PriceString { get; set; }
        public GuardType GuardType{ get; set; }

        public int EffectId { get; set; }

        public int TargetGuardCount { get; set; }
    }
}
