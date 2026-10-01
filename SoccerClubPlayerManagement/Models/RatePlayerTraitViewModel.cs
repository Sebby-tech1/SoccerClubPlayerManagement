using System.ComponentModel.DataAnnotations;

namespace SoccerClubPlayerManagement.Models
{
    public class RatePlayerTraitsViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;

        [Display(Name = "Preferred Foot")]
        public PreferredFoot? PreferredFoot { get; set; }

        [Range(3, 8)]
        [Display(Name = "Height (ft)")]
        public int? HeightFeet { get; set; }

        [Range(0, 11)]
        [Display(Name = "Height (in)")]
        public int? HeightInches { get; set; }

        public List<TraitRatingItem> Traits { get; set; } = new();
    }

    public class TraitRatingItem
    {
        public int TraitId { get; set; }
        public string TraitName { get; set; } = string.Empty;

        // Null/0 means "not rated" — the controller removes the PlayerTrait row in that case.
        public int? Rating { get; set; }
    }
}
