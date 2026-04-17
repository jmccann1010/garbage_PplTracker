namespace PplTracker.Core.Models;

public class PersonLocation
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public int LocationId { get; set; }
    public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
    public DateTime? CheckOutTime { get; set; }
    public string? Notes { get; set; }

    public Person Person { get; set; } = null!;
    public Location Location { get; set; } = null!;
}
