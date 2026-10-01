namespace SoccerClubPlayerManagement.Models
{
    public class EditLineupViewModel
    {
        public int LineupId { get; set; }
        public int FormationId { get; set; }

        // Must be one of the 11 players assigned in Slots — validated in the controller.
        public int? CaptainPlayerId { get; set; }

        public List<LineupSlotAssignment> Slots { get; set; } = new();
        public List<SubstituteAssignment> Substitutes { get; set; } = new();
    }

    public class LineupSlotAssignment
    {
        public int FormationSlotId { get; set; }
        public string Label { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public int? PlayerId { get; set; }
    }

    public class SubstituteAssignment
    {
        public int? PlayerId { get; set; }
    }
}
