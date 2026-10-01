using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoccerClubPlayerManagement.Models
{
    // Many-to-many join between Player and Trait, carrying a per-player rating.
    public class PlayerTrait
    {
        public int PlayerTraitId { get; set; }

        [ForeignKey(nameof(Player))]
        public int PlayerId { get; set; }
        public Player? Player { get; set; }

        [ForeignKey(nameof(Trait))]
        public int TraitId { get; set; }
        public Trait? Trait { get; set; }

        [Range(1, 10)]
        public int Rating { get; set; } // 1-10 scale, set/updated by coaching staff
    }
}
