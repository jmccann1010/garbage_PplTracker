namespace PplTracker.Core.Models;

public class Person
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PersonLocation> PersonLocations { get; set; } = new List<PersonLocation>();
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public string FullName => $"{FirstName} {LastName}".Trim();
}
