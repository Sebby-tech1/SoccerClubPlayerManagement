using System.ComponentModel.DataAnnotations;

namespace SoccerClubPlayerManagement.Models
{
    public class Position
    {
        public int PositionId { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty; // e.g. Goalkeeper, Defender, Midfielder, Forward

        public string? Description { get; set; }

        public ICollection<Player>? Players { get; set; }
    }
}
