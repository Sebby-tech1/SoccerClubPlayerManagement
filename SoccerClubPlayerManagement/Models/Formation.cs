using System.ComponentModel.DataAnnotations;

namespace SoccerClubPlayerManagement.Models
{
    public class Formation
    {
        public int FormationId { get; set; }

        [Required, StringLength(20)]
        public string Name { get; set; } = string.Empty; // e.g. "4-4-2", "4-3-3", "3-5-2","4-2-3-1

        public ICollection<FormationSlot>? Slots { get; set; }
    }
}
