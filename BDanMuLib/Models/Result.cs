using BDanmuLib.Models;

namespace BDanMuLib.Models
{
    public class Result
    {
        public MessageType Type { get; set; }
        public BaseInfo Info { get; set; }

        public Result(MessageType type, BaseInfo info)
        {
            Type = type;
            Info = info;
        }

        public static Result Default => new(MessageType.NONE, null);
    }
}
