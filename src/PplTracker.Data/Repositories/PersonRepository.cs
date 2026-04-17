using Microsoft.EntityFrameworkCore;
using PplTracker.Core.Interfaces;
using PplTracker.Core.Models;
using PplTracker.Data.Context;

namespace PplTracker.Data.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly PplTrackerDbContext _context;

    public PersonRepository(PplTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Person>> GetAllAsync()
    {
        return await _context.People
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task<Person?> GetByIdAsync(int id)
    {
        return await _context.People
            .Include(p => p.PersonLocations)
                .ThenInclude(pl => pl.Location)
            .Include(p => p.Schedules)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Person> CreateAsync(Person entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.People.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Person> UpdateAsync(Person entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.People.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var person = await _context.People.FindAsync(id);
        if (person == null) return false;
        _context.People.Remove(person);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.People.AnyAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Person>> SearchAsync(string searchTerm)
    {
        var lower = searchTerm.ToLower();
        return await _context.People
            .Where(p => p.FirstName.ToLower().Contains(lower)
                     || p.LastName.ToLower().Contains(lower)
                     || (p.Email != null && p.Email.ToLower().Contains(lower)))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Person>> GetPeopleAtLocationAsync(int locationId)
    {
        return await _context.PersonLocations
            .Where(pl => pl.LocationId == locationId && pl.CheckOutTime == null)
            .Include(pl => pl.Person)
            .Select(pl => pl.Person)
            .Distinct()
            .ToListAsync();
    }
}
