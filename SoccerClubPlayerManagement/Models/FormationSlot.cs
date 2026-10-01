using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoccerClubPlayerManagement.Models
{
    public class FormationSlot
    {
        public int FormationSlotId { get; set; }

        [ForeignKey(nameof(Formation))]
        public int FormationId { get; set; }
        public Formation? Formation { get; set; }

        [Required, StringLength(10)]
        public string Label { get; set; } = string.Empty; // e.g. "GK", "LB", "CB", "ST"

        public int SlotOrder { get; set; }

        // Percentage-based pitch coordinates (0-100) for positioning a marker on the diagram.
        // X: 0 = left touchline, 100 = right touchline. Y: 0 = attacking end, 100 = own goal.
        public int X { get; set; }
        public int Y { get; set; }
    }
}
