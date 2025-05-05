namespace events_coordination_frontend.DTOs;

public class EventDTO
{
    public string Title { get; set; }
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }
    public TimeSpan StartTime { get; set; }

    public DateTime EndDate { get; set; }
    public TimeSpan EndTime { get; set; }

    public int VenueId { get; set; }
}
