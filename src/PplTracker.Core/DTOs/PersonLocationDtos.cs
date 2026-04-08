using System.ComponentModel.DataAnnotations;

namespace PplTracker.Core.DTOs;

public class PersonLocationDto
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? Notes { get; set; }
}

public class CreatePersonLocationDto
{
    [Required]
    public int PersonId { get; set; }

    [Required]
    public int LocationId { get; set; }

    public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
    public DateTime? CheckOutTime { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
