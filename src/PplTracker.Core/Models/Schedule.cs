namespace PplTracker.Core.Models;

public class Schedule
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsAllDay { get; set; }
    public int PersonId { get; set; }
    public int? LocationId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Person Person { get; set; } = null!;
    public Location? Location { get; set; }
}
