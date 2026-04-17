using Microsoft.AspNetCore.Mvc;
using PplTracker.Core.DTOs;
using PplTracker.Core.Interfaces;
using PplTracker.Core.Models;

namespace PplTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleRepository _repository;
    private readonly IPersonRepository _personRepository;
    private readonly ILocationRepository _locationRepository;

    public SchedulesController(
        IScheduleRepository repository,
        IPersonRepository personRepository,
        ILocationRepository locationRepository)
    {
        _repository = repository;
        _personRepository = personRepository;
        _locationRepository = locationRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetAll()
    {
        var schedules = await _repository.GetAllAsync();
        return Ok(schedules.Select(MapToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ScheduleDto>> GetById(int id)
    {
        var schedule = await _repository.GetByIdAsync(id);
        if (schedule == null) return NotFound();
        return Ok(MapToDto(schedule));
    }

    [HttpGet("person/{personId:int}")]
    public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetByPerson(int personId)
    {
        var schedules = await _repository.GetByPersonAsync(personId);
        return Ok(schedules.Select(MapToDto));
    }

    [HttpGet("location/{locationId:int}")]
    public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetByLocation(int locationId)
    {
        var schedules = await _repository.GetByLocationAsync(locationId);
        return Ok(schedules.Select(MapToDto));
    }

    [HttpGet("range")]
    public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetByDateRange(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        var schedules = await _repository.GetByDateRangeAsync(start, end);
        return Ok(schedules.Select(MapToDto));
    }

    [HttpPost]
    public async Task<ActionResult<ScheduleDto>> Create([FromBody] CreateScheduleDto dto)
    {
        if (!await _personRepository.ExistsAsync(dto.PersonId))
            return BadRequest($"Person with ID {dto.PersonId} not found.");

        if (dto.LocationId.HasValue && !await _locationRepository.ExistsAsync(dto.LocationId.Value))
            return BadRequest($"Location with ID {dto.LocationId} not found.");

        var schedule = new Schedule
        {
            Title = dto.Title,
            Description = dto.Description,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            IsAllDay = dto.IsAllDay,
            PersonId = dto.PersonId,
            LocationId = dto.LocationId
        };

        var created = await _repository.CreateAsync(schedule);
        var result = await _repository.GetByIdAsync(created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(result!));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ScheduleDto>> Update(int id, [FromBody] UpdateScheduleDto dto)
    {
        var schedule = await _repository.GetByIdAsync(id);
        if (schedule == null) return NotFound();

        if (!await _personRepository.ExistsAsync(dto.PersonId))
            return BadRequest($"Person with ID {dto.PersonId} not found.");

        if (dto.LocationId.HasValue && !await _locationRepository.ExistsAsync(dto.LocationId.Value))
            return BadRequest($"Location with ID {dto.LocationId} not found.");

        schedule.Title = dto.Title;
        schedule.Description = dto.Description;
        schedule.StartTime = dto.StartTime;
        schedule.EndTime = dto.EndTime;
        schedule.IsAllDay = dto.IsAllDay;
        schedule.PersonId = dto.PersonId;
        schedule.LocationId = dto.LocationId;

        var updated = await _repository.UpdateAsync(schedule);
        var result = await _repository.GetByIdAsync(updated.Id);
        return Ok(MapToDto(result!));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    private static ScheduleDto MapToDto(Schedule schedule) => new()
    {
        Id = schedule.Id,
        Title = schedule.Title,
        Description = schedule.Description,
        StartTime = schedule.StartTime,
        EndTime = schedule.EndTime,
        IsAllDay = schedule.IsAllDay,
        PersonId = schedule.PersonId,
        PersonName = schedule.Person?.FullName ?? string.Empty,
        LocationId = schedule.LocationId,
        LocationName = schedule.Location?.Name
    };
}
