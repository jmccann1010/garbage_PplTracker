using Microsoft.EntityFrameworkCore;
using PplTracker.Core.Interfaces;
using PplTracker.Core.Models;
using PplTracker.Data.Context;

namespace PplTracker.Data.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly PplTrackerDbContext _context;

    public ScheduleRepository(PplTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Schedule>> GetAllAsync()
    {
        return await _context.Schedules
            .Include(s => s.Person)
            .Include(s => s.Location)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<Schedule?> GetByIdAsync(int id)
    {
        return await _context.Schedules
            .Include(s => s.Person)
            .Include(s => s.Location)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Schedule> CreateAsync(Schedule entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Schedules.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Schedule> UpdateAsync(Schedule entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Schedules.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var schedule = await _context.Schedules.FindAsync(id);
        if (schedule == null) return false;
        _context.Schedules.Remove(schedule);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Schedules.AnyAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Schedule>> GetByPersonAsync(int personId)
    {
        return await _context.Schedules
            .Include(s => s.Person)
            .Include(s => s.Location)
            .Where(s => s.PersonId == personId)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetByLocationAsync(int locationId)
    {
        return await _context.Schedules
            .Include(s => s.Person)
            .Include(s => s.Location)
            .Where(s => s.LocationId == locationId)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetByDateRangeAsync(DateTime start, DateTime end)
    {
        return await _context.Schedules
            .Include(s => s.Person)
            .Include(s => s.Location)
            .Where(s => s.StartTime >= start && s.StartTime <= end)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }
}
