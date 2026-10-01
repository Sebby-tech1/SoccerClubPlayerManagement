using System.ComponentModel.DataAnnotations.Schema;

namespace SoccerClubPlayerManagement.Models
{
    public class LineupSlot
    {
        public int LineupSlotId { get; set; }

        [ForeignKey(nameof(Lineup))]
        public int LineupId { get; set; }
        public Lineup? Lineup { get; set; }

        [ForeignKey(nameof(FormationSlot))]
        public int FormationSlotId { get; set; }
        public FormationSlot? FormationSlot { get; set; }

        // Nullable: a slot can be saved unfilled if the coach hasn't picked someone for it yet.
        [ForeignKey(nameof(Player))]
        public int? PlayerId { get; set; }
        public Player? Player { get; set; }
    }
}
