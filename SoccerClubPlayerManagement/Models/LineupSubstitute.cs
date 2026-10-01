using System.ComponentModel.DataAnnotations.Schema;

namespace SoccerClubPlayerManagement.Models
{
    public class LineupSubstitute
    {
        public int LineupSubstituteId { get; set; }

        [ForeignKey(nameof(Lineup))]
        public int LineupId { get; set; }
        public Lineup? Lineup { get; set; }

        [ForeignKey(nameof(Player))]
        public int? PlayerId { get; set; }
        public Player? Player { get; set; }

        // Bench order (1-7) — purely for consistent display, not a formal substitution priority.
        public int SubOrder { get; set; }
    }
}
