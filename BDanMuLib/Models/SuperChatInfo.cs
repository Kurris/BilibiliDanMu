
namespace BDanMuLib.Models
{
    public class SuperChatInfo : BaseInfo
    {
        public string BackgroundColor { get; set; }


        public string UserFace { get; set; }
        public string UserFaceFrame { get; set; }
        public string UserName { get; set; }
        public string UserNameColor { get; set; }


        public bool HasMedal => !string.IsNullOrEmpty(MedalName);
        public string MedalColor { get; set; }
        public string MedalName { get; set; }
        public int MedalLevel { get; set; }


        public string BackgroundImage { get; set; }
        public string BackgroundPriceColor { get; set; }
        public string BackgroundBottomColor { get; set; }
        public string MessageFontColor { get; set; }


        public int Price { get; set; }
        public string Message { get; set; }
        public int Num { get; set; }

        public int GiftId { get; set; }
        public string GiftName { get; set; }


        public string SpeakText => string.Concat(UserName, "发送了", Num, "条", GiftName, ",", Message);
    }
}
