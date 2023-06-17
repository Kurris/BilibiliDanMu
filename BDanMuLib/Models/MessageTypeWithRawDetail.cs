using LiveCore.Enums;

namespace LiveCore.Models
{
    public class MessageTypeWithRawDetail
    {
        public MessageType Type { get; set; }
        public string Raw { get; set; }
    }
}
