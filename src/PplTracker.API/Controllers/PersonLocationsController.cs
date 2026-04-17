using Microsoft.AspNetCore.Mvc;
using PplTracker.Core.DTOs;
using PplTracker.Core.Interfaces;
using PplTracker.Core.Models;

namespace PplTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonLocationsController : ControllerBase
{
    private readonly IPersonLocationRepository _repository;
    private readonly IPersonRepository _personRepository;
    private readonly ILocationRepository _locationRepository;

    public PersonLocationsController(
        IPersonLocationRepository repository,
        IPersonRepository personRepository,
        ILocationRepository locationRepository)
    {
        _repository = repository;
        _personRepository = personRepository;
        _locationRepository = locationRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonLocationDto>>> GetAll()
    {
        var items = await _repository.GetAllAsync();
        return Ok(items.Select(MapToDto));
    }

    [HttpGet("current")]
    public async Task<ActionResult<IEnumerable<PersonLocationDto>>> GetCurrentlyCheckedIn()
    {
        var items = await _repository.GetCurrentlyCheckedInAsync();
        return Ok(items.Select(MapToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PersonLocationDto>> GetById(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(MapToDto(item));
    }

    [HttpGet("person/{personId:int}")]
    public async Task<ActionResult<IEnumerable<PersonLocationDto>>> GetByPerson(int personId)
    {
        var items = await _repository.GetByPersonAsync(personId);
        return Ok(items.Select(MapToDto));
    }

    [HttpGet("location/{locationId:int}")]
    public async Task<ActionResult<IEnumerable<PersonLocationDto>>> GetByLocation(int locationId)
    {
        var items = await _repository.GetByLocationAsync(locationId);
        return Ok(items.Select(MapToDto));
    }

    [HttpPost]
    public async Task<ActionResult<PersonLocationDto>> CheckIn([FromBody] CreatePersonLocationDto dto)
    {
        if (!await _personRepository.ExistsAsync(dto.PersonId))
            return BadRequest($"Person with ID {dto.PersonId} not found.");

        if (!await _locationRepository.ExistsAsync(dto.LocationId))
            return BadRequest($"Location with ID {dto.LocationId} not found.");

        var personLocation = new PersonLocation
        {
            PersonId = dto.PersonId,
            LocationId = dto.LocationId,
            CheckInTime = dto.CheckInTime,
            CheckOutTime = dto.CheckOutTime,
            Notes = dto.Notes
        };

        var created = await _repository.CreateAsync(personLocation);
        var result = await _repository.GetByIdAsync(created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(result!));
    }

    [HttpPut("{id:int}/checkout")]
    public async Task<ActionResult<PersonLocationDto>> CheckOut(int id)
    {
        var result = await _repository.CheckOutAsync(id);
        if (result == null) return NotFound();
        var item = await _repository.GetByIdAsync(id);
        return Ok(MapToDto(item!));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    private static PersonLocationDto MapToDto(PersonLocation pl) => new()
    {
        Id = pl.Id,
        PersonId = pl.PersonId,
        PersonName = pl.Person?.FullName ?? string.Empty,
        LocationId = pl.LocationId,
        LocationName = pl.Location?.Name ?? string.Empty,
        CheckInTime = pl.CheckInTime,
        CheckOutTime = pl.CheckOutTime,
        Notes = pl.Notes
    };
}
