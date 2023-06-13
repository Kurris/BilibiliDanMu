using System;
using BDanMuLib.Emuns;
using Microsoft.VisualBasic;

namespace BDanMuLib.Emuns
{
    /// <summary>
    /// 舰长类型
    /// </summary>
    public class GuardType : BaseEnumeration
    {
        public static GuardType 白嫖的观众 = new(0, "白嫖的观众", "white", string.Empty, string.Empty);

        public static GuardType 总督 = new(1, "总督", "#F82818",
            "https://i0.hdslb.com/bfs/live/39164ebfdd39db3d284b1221765e7e57f5a49958.png",
            "https://s1.hdslb.com/bfs/static/blive/blfe-live-room/static/img/icon-l-1.fde1190..png");

        public static GuardType 提督 = new(2, "提督", "#E17AFF",
            "https://i0.hdslb.com/bfs/live/09937c3beb0608e267a50ac3c7125c3f2d709098.png",
            "https://s1.hdslb.com/bfs/static/blive/blfe-live-room/static/img/icon-l-2.6f68d77..png");

        public static GuardType 舰长 = new(3, "舰长", "#00D1F1",
            "https://i0.hdslb.com/bfs/live/80f732943cc3367029df65e267960d56736a82ee.png",
            "https://s1.hdslb.com/bfs/static/blive/blfe-live-room/static/img/icon-l-3.402ac8f..png");

        /// <summary>
        /// 星星frame
        /// </summary>
        public string StarFrameUrl => "https://s1.hdslb.com/bfs/static/blive/blfe-live-room/static/img/star.d40d9a4..png";

        public GuardType(int id, string name, string fontColor, string frameUrl, string fansMedalIconUrl) : base(id, name)
        {
            this.FontColor = fontColor;
            this.FrameUrl = frameUrl;
        }

        public int Level => Id;

        /// <summary>
        /// 字体颜色
        ///   舰长
        ///   #E17AFF 提督
        ///   没有总督大哥测试 
        /// </summary>
        public string FontColor { get; set; }

        /// <summary>
        /// 头像边框
        /// </summary>
        public string FrameUrl { get; set; }

        /// <summary>
        /// 粉丝牌icon
        /// </summary>
        public string FansMedalIconUrl { get; set; }

        public static GuardType CheckGuardByColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return GuardType.白嫖的观众;
            if (color.Equals("#00D1F1")) return GuardType.舰长;
            if (color.Equals("#E17AFF")) return GuardType.提督;
            return GuardType.总督; // 自定义的#F82818,没有总督大哥的数据
        }
    }
}

