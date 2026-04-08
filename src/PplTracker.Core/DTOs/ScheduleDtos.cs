using System.ComponentModel.DataAnnotations;

namespace PplTracker.Core.DTOs;

public class ScheduleDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsAllDay { get; set; }
    public int PersonId { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public int? LocationId { get; set; }
    public string? LocationName { get; set; }
}

public class CreateScheduleDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public bool IsAllDay { get; set; }

    [Required]
    public int PersonId { get; set; }

    public int? LocationId { get; set; }
}

public class UpdateScheduleDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public bool IsAllDay { get; set; }

    [Required]
    public int PersonId { get; set; }

    public int? LocationId { get; set; }
}
