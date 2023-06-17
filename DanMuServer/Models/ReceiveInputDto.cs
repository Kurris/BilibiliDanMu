using System.ComponentModel.DataAnnotations;

namespace LiveServer.Models;

public class ReceiveInputDto
{

    [Required]
    public string ConnectionId { get; set; }

    [Range(1, int.MaxValue)]
    public int RoomId { get; set; }
}