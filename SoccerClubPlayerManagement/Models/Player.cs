using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoccerClubPlayerManagement.Models
{
    public class Player
    {
        public int PlayerId { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Phone]
        [Display(Name = "Contact Number")]
        public string? ContactNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [ForeignKey(nameof(Position))]
        [Display(Name = "Position")]
        public int PositionId { get; set; }
        public Position? Position { get; set; }

        // Availability for selection, tracked by coaching staff
        [Display(Name = "Available for Selection")]
        public bool IsAvailable { get; set; } = true;

        // Physical/skill attributes set by coaches on the Rate Traits page (null until recorded)
        [Display(Name = "Preferred Foot")]
        public PreferredFoot? PreferredFoot { get; set; }

        [Range(3, 8)]
        [Display(Name = "Height (ft)")]
        public int? HeightFeet { get; set; }

        [Range(0, 11)]
        [Display(Name = "Height (in)")]
        public int? HeightInches { get; set; }

        // Soft delete: retains player info after they leave the team, per client requirement
        public bool IsActive { get; set; } = true;

        // URL/path to the photo once stored (Azure Blob Storage or wwwroot in dev)
        public string? PhotoUrl { get; set; }

        // Not persisted directly — bound from the upload form, handled in the controller
        [NotMapped]
        [Display(Name = "Player Photo")]
        public IFormFile? PhotoFile { get; set; }

        public ICollection<PlayerTrait>? PlayerTraits { get; set; }
    }
}
