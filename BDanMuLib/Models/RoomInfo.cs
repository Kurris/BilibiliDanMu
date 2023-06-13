
using System;
using System.Collections.Generic;

namespace BDanMuLib.Models
{
    public class RoomInfo : BaseInfo
    {
        public long Uid { get; set; }

        public int RoomId { get; set; }
        public int ShortRoomId { get; set; }


        public string Title { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public string Description { get; set; }


        public int AreaId { get; set; }
        public string AreaName { get; set; }


        public int ParentAreaId { get; set; }
        public string ParentAreaName { get; set; }

        public string KeyFrameUrl { get; set; }
        
        public string UserCoverUrl { get; set; }
        public string BackgroundUrl { get; set; }

        public DateTime LiveTime { get; set; }

        /// <summary>
        /// 0：未开播 1：直播中 2：轮播中
        /// </summary>
        public int LiveStatus { get; set; }
    }
}
