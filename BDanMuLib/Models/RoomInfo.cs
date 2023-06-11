
using System.Collections.Generic;

namespace BDanMuLib.Models
{
    public class RoomInfo : BaseInfo
    {
        public int RoomId { get; set; }
        public int ShortRoomId { get; set; }


        public string Title { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public string Description { get; set; }


        public int AreaId { get; set; }
        public string AreaName { get; set; }


        public int ParentAreaId { get; set; }
        public string ParentAreaName { get; set; }

        /// <summary>
        /// 0：未开播 1：直播中 2：轮播中
        /// </summary>
        public int LiveStatus { get; set; }
    }
}
