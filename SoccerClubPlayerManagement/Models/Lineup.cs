using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoccerClubPlayerManagement.Models
{
    public class Lineup
    {
        public int LineupId { get; set; }

        [StringLength(50)]
        public string Name { get; set; } = "Starting XI";

        [ForeignKey(nameof(Formation))]
        public int FormationId { get; set; }
        public Formation? Formation { get; set; }

        // Must be one of the 11 starting players — enforced in the controller, not the database.
        [ForeignKey(nameof(Captain))]
        public int? CaptainPlayerId { get; set; }
        public Player? Captain { get; set; }

        // Snapshot of the logged-in coach's name at the time the lineup was saved.
        [StringLength(100)]
        public string? CoachName { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public ICollection<LineupSlot>? LineupSlots { get; set; }
        public ICollection<LineupSubstitute>? Substitutes { get; set; }
    }
}
