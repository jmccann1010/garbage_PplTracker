using Microsoft.EntityFrameworkCore;
using PplTracker.Core.Interfaces;
using PplTracker.Core.Models;
using PplTracker.Data.Context;

namespace PplTracker.Data.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly PplTrackerDbContext _context;

    public LocationRepository(PplTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Location>> GetAllAsync()
    {
        return await _context.Locations
            .OrderBy(l => l.Name)
            .ToListAsync();
    }

    public async Task<Location?> GetByIdAsync(int id)
    {
        return await _context.Locations
            .Include(l => l.PersonLocations)
                .ThenInclude(pl => pl.Person)
            .Include(l => l.Schedules)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Location> CreateAsync(Location entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Locations.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Location> UpdateAsync(Location entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Locations.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null) return false;
        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Locations.AnyAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<Location>> SearchAsync(string searchTerm)
    {
        var lower = searchTerm.ToLower();
        return await _context.Locations
            .Where(l => l.Name.ToLower().Contains(lower)
                     || (l.City != null && l.City.ToLower().Contains(lower))
                     || (l.Address != null && l.Address.ToLower().Contains(lower)))
            .OrderBy(l => l.Name)
            .ToListAsync();
    }
}
