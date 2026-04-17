using Microsoft.AspNetCore.Mvc;
using PplTracker.Core.DTOs;
using PplTracker.Core.Interfaces;
using PplTracker.Core.Models;

namespace PplTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationRepository _repository;

    public LocationsController(ILocationRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LocationDto>>> GetAll()
    {
        var locations = await _repository.GetAllAsync();
        return Ok(locations.Select(MapToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LocationDto>> GetById(int id)
    {
        var location = await _repository.GetByIdAsync(id);
        if (location == null) return NotFound();
        return Ok(MapToDto(location));
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<LocationDto>>> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q)) return BadRequest("Search term is required.");
        var locations = await _repository.SearchAsync(q);
        return Ok(locations.Select(MapToDto));
    }

    [HttpPost]
    public async Task<ActionResult<LocationDto>> Create([FromBody] CreateLocationDto dto)
    {
        var location = new Location
        {
            Name = dto.Name,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            ZipCode = dto.ZipCode,
            Country = dto.Country,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Notes = dto.Notes
        };

        var created = await _repository.CreateAsync(location);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<LocationDto>> Update(int id, [FromBody] UpdateLocationDto dto)
    {
        var location = await _repository.GetByIdAsync(id);
        if (location == null) return NotFound();

        location.Name = dto.Name;
        location.Address = dto.Address;
        location.City = dto.City;
        location.State = dto.State;
        location.ZipCode = dto.ZipCode;
        location.Country = dto.Country;
        location.Latitude = dto.Latitude;
        location.Longitude = dto.Longitude;
        location.Notes = dto.Notes;

        var updated = await _repository.UpdateAsync(location);
        return Ok(MapToDto(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    private static LocationDto MapToDto(Location location) => new()
    {
        Id = location.Id,
        Name = location.Name,
        Address = location.Address,
        City = location.City,
        State = location.State,
        ZipCode = location.ZipCode,
        Country = location.Country,
        Latitude = location.Latitude,
        Longitude = location.Longitude,
        Notes = location.Notes
    };
}
