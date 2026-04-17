using Microsoft.EntityFrameworkCore;
using PplTracker.Core.Interfaces;
using PplTracker.Core.Models;
using PplTracker.Data.Context;

namespace PplTracker.Data.Repositories;

public class PersonLocationRepository : IPersonLocationRepository
{
    private readonly PplTrackerDbContext _context;

    public PersonLocationRepository(PplTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PersonLocation>> GetAllAsync()
    {
        return await _context.PersonLocations
            .Include(pl => pl.Person)
            .Include(pl => pl.Location)
            .OrderByDescending(pl => pl.CheckInTime)
            .ToListAsync();
    }

    public async Task<PersonLocation?> GetByIdAsync(int id)
    {
        return await _context.PersonLocations
            .Include(pl => pl.Person)
            .Include(pl => pl.Location)
            .FirstOrDefaultAsync(pl => pl.Id == id);
    }

    public async Task<PersonLocation> CreateAsync(PersonLocation entity)
    {
        _context.PersonLocations.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<PersonLocation> UpdateAsync(PersonLocation entity)
    {
        _context.PersonLocations.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var personLocation = await _context.PersonLocations.FindAsync(id);
        if (personLocation == null) return false;
        _context.PersonLocations.Remove(personLocation);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.PersonLocations.AnyAsync(pl => pl.Id == id);
    }

    public async Task<IEnumerable<PersonLocation>> GetByPersonAsync(int personId)
    {
        return await _context.PersonLocations
            .Include(pl => pl.Person)
            .Include(pl => pl.Location)
            .Where(pl => pl.PersonId == personId)
            .OrderByDescending(pl => pl.CheckInTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<PersonLocation>> GetByLocationAsync(int locationId)
    {
        return await _context.PersonLocations
            .Include(pl => pl.Person)
            .Include(pl => pl.Location)
            .Where(pl => pl.LocationId == locationId)
            .OrderByDescending(pl => pl.CheckInTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<PersonLocation>> GetCurrentlyCheckedInAsync()
    {
        return await _context.PersonLocations
            .Include(pl => pl.Person)
            .Include(pl => pl.Location)
            .Where(pl => pl.CheckOutTime == null)
            .OrderByDescending(pl => pl.CheckInTime)
            .ToListAsync();
    }

    public async Task<PersonLocation?> CheckOutAsync(int personLocationId)
    {
        var personLocation = await _context.PersonLocations.FindAsync(personLocationId);
        if (personLocation == null) return null;
        personLocation.CheckOutTime = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return personLocation;
    }
}
