using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoccerClubPlayerManagement.Models
{
    public class Trait
    {
        public int TraitId { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty; // e.g. Pace, Stamina, Leadership, Finishing

        public string? Description { get; set; }

        // Null = applies to every player. Set = only rateable for players in that position
        // (e.g. Diving/Reflexes/Handling/Distribution only apply to Goalkeeper).
        [ForeignKey(nameof(Position))]
        public int? PositionId { get; set; }
        public Position? Position { get; set; }

        public ICollection<PlayerTrait>? PlayerTraits { get; set; }
    }
}
