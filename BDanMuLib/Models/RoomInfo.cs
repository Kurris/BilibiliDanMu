
namespace BDanMuLib.Models
{
    public class RoomInfo : BaseInfo
    {
        public int RoomId { get; set; }
        public int ShortRoomId { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public int ParentAreaId { get; set; }
        public string ParentAreaName { get; set; }
    }
}
