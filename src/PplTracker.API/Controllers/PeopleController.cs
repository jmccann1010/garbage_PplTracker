using Microsoft.AspNetCore.Mvc;
using PplTracker.Core.DTOs;
using PplTracker.Core.Interfaces;
using PplTracker.Core.Models;

namespace PplTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController : ControllerBase
{
    private readonly IPersonRepository _repository;

    public PeopleController(IPersonRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetAll()
    {
        var people = await _repository.GetAllAsync();
        return Ok(people.Select(MapToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PersonDto>> GetById(int id)
    {
        var person = await _repository.GetByIdAsync(id);
        if (person == null) return NotFound();
        return Ok(MapToDto(person));
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<PersonDto>>> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q)) return BadRequest("Search term is required.");
        var people = await _repository.SearchAsync(q);
        return Ok(people.Select(MapToDto));
    }

    [HttpGet("{id:int}/location/{locationId:int}")]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetPeopleAtLocation(int locationId)
    {
        var people = await _repository.GetPeopleAtLocationAsync(locationId);
        return Ok(people.Select(MapToDto));
    }

    [HttpPost]
    public async Task<ActionResult<PersonDto>> Create([FromBody] CreatePersonDto dto)
    {
        var person = new Person
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Notes = dto.Notes
        };

        var created = await _repository.CreateAsync(person);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PersonDto>> Update(int id, [FromBody] UpdatePersonDto dto)
    {
        var person = await _repository.GetByIdAsync(id);
        if (person == null) return NotFound();

        person.FirstName = dto.FirstName;
        person.LastName = dto.LastName;
        person.Email = dto.Email;
        person.Phone = dto.Phone;
        person.Notes = dto.Notes;

        var updated = await _repository.UpdateAsync(person);
        return Ok(MapToDto(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    private static PersonDto MapToDto(Person person) => new()
    {
        Id = person.Id,
        FirstName = person.FirstName,
        LastName = person.LastName,
        Email = person.Email,
        Phone = person.Phone,
        Notes = person.Notes
    };
}
