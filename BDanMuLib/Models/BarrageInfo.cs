using System;
using BDanMuLib.Emuns;

namespace BDanMuLib.Models
{
    public class BarrageInfo : BaseInfo
    {
        public string Mid { get; set; }
        public string FaceUrl { get; set; }
        public string Comment { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime Time { get; set; }
        public string UserName { get; set; }
        public int AudRank { get; set; }
        public bool HasMedal { get; set; }
        public string MedalName { get; set; }
        public int MedalLevel { get; set; }
        public int Top3 { get; set; }
        public GuardType Guard { get; set; }

    }
}
